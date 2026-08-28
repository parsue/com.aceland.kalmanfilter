# Burst, Jobs & ECS

Because a filter is a blittable `struct` with no managed references and no virtual dispatch, it drops
straight into the Jobs system and ECS. This page shows how to run thousands of filters in parallel and how
to keep everything Burst-compiled.

## The Golden Rule
Always work with the **concrete** filter type, `KalmanFilter<T, TAdapter>`. Never store or pass it as the
`IKalmanFilter<T>` interface — doing so boxes the struct onto the managed heap and disables both Burst and
the allocation-free guarantee.

```csharp
// ✅ Burst-friendly: concrete struct, lives on the stack / in a NativeArray.
KalmanFilter<float3, Float3Adapter> good = Kalman.CreateFloat3Filter();

// ❌ Boxed onto the heap: loses Burst and allocates.
IKalmanFilter<float3> bad = Kalman.CreateFloat3Filter();
```

---

## KalmanBatchJob — filter thousands in parallel
The package ships a ready-to-use `IJobParallelFor` that runs one independent filter per element. Give it a
`NativeArray` of filters, a flat buffer of measurements, and a results array.

```csharp
using Aceland.KalmanFilter;
using Aceland.KalmanFilter.Jobs;
using Unity.Collections;
using Unity.Jobs;

public sealed class BatchFilterExample
{
    public NativeArray<float> Run(int signalCount, int samplesPerFilter, NativeArray<float> measurements)
    {
        // One filter per signal.
        var filters = new NativeArray<KalmanFilter<float, FloatAdapter>>(signalCount, Allocator.TempJob);
        for (var i = 0; i < filters.Length; i++)
            filters[i] = Kalman.CreateFloatFilter(r: 1e-2f);

        var results = new NativeArray<float>(signalCount, Allocator.TempJob);

        var job = new KalmanBatchJob<float, FloatAdapter>
        {
            Filters          = filters,       // mutated in place with the updated state
            Measurements     = measurements,  // length == signalCount * samplesPerFilter
            Results          = results,       // final estimate per signal
            SamplesPerFilter = samplesPerFilter,
        };

        // 64 = inner-loop batch count; tune for your workload.
        job.Schedule(signalCount, 64).Complete();

        filters.Dispose();
        return results; // caller disposes
    }
}
```

{% hint style="info" %}
`Measurements` is a **flat** buffer laid out as `[signalIndex * SamplesPerFilter + sampleIndex]`. Each job
index reads its own contiguous row, so there is no cross-thread contention.
{% endhint %}

The job body itself is trivial — it just pulls the filter out of the array, runs it over its row of
samples, and writes both the updated filter and the final estimate back:

```csharp
public void Execute(int index)
{
    var filter = Filters[index];
    var start = index * SamplesPerFilter;

    var last = filter.GetCurrentValues().x;
    for (var i = 0; i < SamplesPerFilter; ++i)
        last = filter.Update(Measurements[start + i]);

    Filters[index] = filter;   // struct copied back so state persists
    Results[index] = last;
}
```

---

## Registering Generic Job Types
Unity's Job system must know every **closed** generic job it will schedule ahead of time. The package
already registers `KalmanBatchJob` for all built-in types:

```csharp
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float,   FloatAdapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector2, Vector2Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector3, Vector3Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector4, Vector4Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float2,  Float2Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float3,  Float3Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float4,  Float4Adapter>))]
```

{% hint style="warning" %}
If you schedule `KalmanBatchJob` (or your own job) with a **custom adapter**, add a matching registration
line in *your own* assembly, otherwise Burst has nothing to compile:

```csharp
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Color, ColorAdapter>))]
```
{% endhint %}

---

## Using Filters on ECS Components
Since the filter is blittable, it can be a field on an `IComponentData` and updated inside a Burst
`ISystem`. No conversion, no boxing.

```csharp
using Aceland.KalmanFilter;
using Unity.Entities;
using Unity.Mathematics;

public struct SmoothedPosition : IComponentData
{
    public KalmanFilter<float3, Float3Adapter> Filter;
}

[BurstCompile]
public partial struct SmoothingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (smoothed, transform) in
                 SystemAPI.Query<RefRW<SmoothedPosition>, RefRW<LocalTransform>>())
        {
            var filtered = smoothed.ValueRW.Filter.Update(transform.ValueRO.Position);
            transform.ValueRW.Position = filtered;
        }
    }
}
```

{% hint style="success" %}
The very same `KalmanFilter<float3, Float3Adapter>` you would use in a `MonoBehaviour` is what sits on the
ECS component here. One implementation, every execution model.
{% endhint %}

---

## Best Practices
- Keep filters in a `NativeArray` (Jobs) or on an `IComponentData` (ECS); both keep them off the GC heap.
- Choose `float2/3/4` for the value type — they Burst-vectorise better than `Vector2/3/4`.
- Remember `Schedule(length, innerBatchCount)` — a batch count of 32–64 is a good starting point.
- Register any custom `KalmanBatchJob<T, TAdapter>` closure with `[assembly: RegisterGenericJobType]`.
- Always `Dispose()` the `NativeArray`s you allocate, or use `Allocator.TempJob` with a completed job.
