using Aceland.KalmanFilter.Adapters;
using Aceland.KalmanFilter.Contracts;
using Aceland.KalmanFilter.Core;
using Aceland.KalmanFilter.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Unity's Job system needs every CLOSED generic job type it will schedule to be registered up-front.
// We register the built-in value types here so users can schedule KalmanBatchJob<T, TAdapter> for them
// without any extra setup. For a fully custom adapter, add your own [assembly: RegisterGenericJobType]
// line in your own assembly.
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float, FloatAdapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector2, Vector2Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector3, Vector3Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<Vector4, Vector4Adapter>))]
#if UNITY_2022_3_OR_NEWER
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float2, Float2Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float3, Float3Adapter>))]
[assembly: RegisterGenericJobType(typeof(KalmanBatchJob<float4, Float4Adapter>))]
#endif

namespace Aceland.KalmanFilter.Jobs
{
    /// <summary>
    /// A ready-to-use Burst job that runs one independent Kalman filter per element in parallel.
    /// Each filter processes its own row of measurements and writes back the final estimate.
    ///
    /// This demonstrates that <see cref="KalmanFilter{T,TAdapter}"/> is fully Burst / Jobs / ECS compatible:
    /// it is a blittable struct with no managed references and no virtual dispatch.
    /// </summary>
    /// <example>
    /// <code>
    /// // Filter 1000 noisy signals in parallel, each with its own stream of samples.
    /// var filters = new NativeArray&lt;KalmanFilter&lt;float, FloatAdapter&gt;&gt;(1000, Allocator.TempJob);
    /// for (var i = 0; i &lt; filters.Length; i++)
    ///     filters[i] = Kalman.CreateFloatFilter();
    ///
    /// var job = new KalmanBatchJob&lt;float, FloatAdapter&gt;
    /// {
    ///     Filters      = filters,
    ///     Measurements = measurements,   // length == filters.Length * SamplesPerFilter
    ///     Results      = results,        // length == filters.Length
    ///     SamplesPerFilter = SamplesPerFilter,
    /// };
    /// job.Schedule(filters.Length, 64).Complete();
    /// </code>
    /// </example>
    /// <typeparam name="T">The unmanaged value type being filtered.</typeparam>
    /// <typeparam name="TAdapter">The value-type adapter for <typeparamref name="T"/>.</typeparam>
    [BurstCompile]
    public struct KalmanBatchJob<T, TAdapter> : IJobParallelFor
        where T : unmanaged
        where TAdapter : unmanaged, IKalmanValueAdapter<T>
    {
        /// <summary>One filter per parallel index. Mutated in place with the updated state.</summary>
        public NativeArray<KalmanFilter<T, TAdapter>> Filters;

        /// <summary>Flat measurement buffer laid out as [index * SamplesPerFilter + sample].</summary>
        [ReadOnly] public NativeArray<T> Measurements;

        /// <summary>Final filtered estimate for each index.</summary>
        [WriteOnly] public NativeArray<T> Results;

        public int SamplesPerFilter;

        public void Execute(int index)
        {
            var filter = Filters[index];
            var start = index * SamplesPerFilter;

            var last = filter.GetCurrentValues().x;
            for (var i = 0; i < SamplesPerFilter; ++i)
                last = filter.Update(Measurements[start + i]);

            Filters[index] = filter;
            Results[index] = last;
        }
    }
}
