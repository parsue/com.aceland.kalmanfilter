using Aceland.KalmanFilter.Adapters;
using Aceland.KalmanFilter.Contracts;
using Aceland.KalmanFilter.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Aceland.KalmanFilter
{
    /// <summary>
    /// Entry point for creating Burst-compatible Kalman filters.
    ///
    /// Every filter returned here is a blittable <c>struct</c>
    /// (<see cref="KalmanFilter{T,TAdapter}"/>), so it can be copied into a Job / ECS component and
    /// executed under Burst with zero managed allocation. Store the returned value directly
    /// (do NOT store it as the <see cref="IKalmanFilter{T}"/> interface if you need Burst, since that boxes it).
    /// </summary>
    public static class Kalman
    {
        // --- Custom value type ------------------------------------------------------------------

        /// <summary>
        /// Creates a filter for any unmanaged type <typeparamref name="T"/> using a custom value adapter.
        /// The adapter type is part of the generic signature so Burst can inline it.
        /// </summary>
        public static KalmanFilter<T, TAdapter> Create<T, TAdapter>(
            TAdapter adapter,
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f)
            where T : unmanaged
            where TAdapter : unmanaged, IKalmanValueAdapter<T> =>
            KalmanFilter<T, TAdapter>.Build(adapter, q, r, p);

        // --- Built-in UnityEngine value types ---------------------------------------------------

        public static KalmanFilter<float, FloatAdapter> CreateFloatFilter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float, FloatAdapter>.Build(default, q, r, p);

        public static KalmanFilter<Vector2, Vector2Adapter> CreateVector2Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector2, Vector2Adapter>.Build(default, q, r, p);

        public static KalmanFilter<Vector3, Vector3Adapter> CreateVector3Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector3, Vector3Adapter>.Build(default, q, r, p);

        public static KalmanFilter<Vector4, Vector4Adapter> CreateVector4Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector4, Vector4Adapter>.Build(default, q, r, p);

        // --- Unity.Mathematics value types (Burst / Jobs / ECS recommended) ---------------------

#if UNITY_2022_3_OR_NEWER
        public static KalmanFilter<float2, Float2Adapter> CreateFloat2Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float2, Float2Adapter>.Build(default, q, r, p);

        public static KalmanFilter<float3, Float3Adapter> CreateFloat3Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float3, Float3Adapter>.Build(default, q, r, p);

        public static KalmanFilter<float4, Float4Adapter> CreateFloat4Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float4, Float4Adapter>.Build(default, q, r, p);
#endif
    }
}
