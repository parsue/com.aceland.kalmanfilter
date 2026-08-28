# Custom Value Adapters

The filter algorithm never mentions `float` or `Vector3` directly. All the type-specific arithmetic lives
in a small **value adapter**, so you can filter *any* `unmanaged` type by writing four one-line methods.

## The Adapter Contract
An adapter implements `IKalmanValueAdapter<T>`. It must be an `unmanaged` value type (a plain `struct`) so
it stays Burst-compatible and gets inlined into the filter.

```csharp
public interface IKalmanValueAdapter<T> where T : unmanaged
{
    T Zero { get; }                     // the identity value (used by Reset)
    T Add(T left, T right);             // left + right
    T Subtract(T left, T right);        // left - right
    T Scale(T value, float scalar);     // value * scalar
}
```

{% hint style="info" %}
That is the whole surface. The Kalman gain, covariance and update loop are handled for you — the adapter
only teaches the filter how to do vector-style math on your type.
{% endhint %}

---

## Built-in Adapters
These ship with the package and are used automatically by the matching factory methods:

| Type | Adapter | Factory |
| --- | --- | --- |
| `float` | `FloatAdapter` | `Kalman.CreateFloatFilter()` |
| `Vector2` | `Vector2Adapter` | `Kalman.CreateVector2Filter()` |
| `Vector3` | `Vector3Adapter` | `Kalman.CreateVector3Filter()` |
| `Vector4` | `Vector4Adapter` | `Kalman.CreateVector4Filter()` |
| `float2` | `Float2Adapter` | `Kalman.CreateFloat2Filter()` |
| `float3` | `Float3Adapter` | `Kalman.CreateFloat3Filter()` |
| `float4` | `Float4Adapter` | `Kalman.CreateFloat4Filter()` |

A typical adapter is tiny — here is the built-in `Vector3` one:

```csharp
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    public readonly struct Vector3Adapter : IKalmanValueAdapter<Vector3>
    {
        public Vector3 Zero => Vector3.zero;
        public Vector3 Add(Vector3 left, Vector3 right) => left + right;
        public Vector3 Subtract(Vector3 left, Vector3 right) => left - right;
        public Vector3 Scale(Vector3 value, float scalar) => value * scalar;
    }
}
```

---

## Writing Your Own
Suppose you want to smooth a `Color` (its four channels are just numbers). Write the adapter, then hand it
to the generic `Kalman.Create` factory.

{% tabs %}
{% tab title="ColorAdapter.cs" %}
```csharp
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

// Mark it [BurstCompile] and make it a readonly struct so it inlines cleanly.
[BurstCompile]
public readonly struct ColorAdapter : IKalmanValueAdapter<Color>
{
    public Color Zero => new(0f, 0f, 0f, 0f);
    public Color Add(Color left, Color right) => left + right;
    public Color Subtract(Color left, Color right) => left - right;
    public Color Scale(Color value, float scalar) => value * scalar;
}
```
{% endtab %}

{% tab title="Using it" %}
```csharp
using Aceland.KalmanFilter;
using UnityEngine;

public class SmoothTint : MonoBehaviour
{
    // The adapter type is part of the filter type, so Burst can inline it.
    private KalmanFilter<Color, ColorAdapter> _filter;

    private void Awake()
    {
        // Pass an adapter instance. A stateless adapter can just be `default`.
        _filter = Kalman.Create<Color, ColorAdapter>(default, r: 2e-2f);
    }

    private void Update()
    {
        Color noisy = SampleColor();
        GetComponent<Renderer>().material.color = _filter.Update(noisy);
    }

    private Color SampleColor() => Color.white;
}
```
{% endtab %}
{% endtabs %}

{% hint style="success" %}
`Kalman.Create<T, TAdapter>(adapter, q, r, p)` is the single entry point for any custom type. The built-in
`CreateFloatFilter()` / `CreateVector3Filter()` helpers are just thin wrappers around it that pass a
`default` adapter for you.
{% endhint %}

---

## Rules to Keep It Burst-Safe
- The adapter **must be an `unmanaged` struct** — no class, no reference fields, no `string`.
- Prefer a **`readonly struct`**; a stateless adapter created with `default` inlines to nothing at runtime.
- Add **`[BurstCompile]`** to the adapter (as the built-ins do) so it compiles inside Burst jobs.
- Keep the four methods **pure and branch-free** where possible; that is what lets Burst vectorise them.

## Best Practices
- Reuse Unity.Mathematics types (`float2/3/4`) inside custom adapters when you can — they Burst-vectorise best.
- If your type needs a non-zero identity, return it from `Zero`; `Reset()` uses exactly that value.
- One adapter per type is enough — it carries no per-filter state, so a single `default` instance serves all filters.
