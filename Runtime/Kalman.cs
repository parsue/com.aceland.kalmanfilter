using Aceland.KalmanFilter.Adapters;
using Aceland.KalmanFilter.Contracts;
using Aceland.KalmanFilter.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Aceland.KalmanFilter
{
    public static class Kalman
    {
        private static readonly FloatAdapter FloatAdapter = new();
        private static readonly Vector2Adapter Vector2Adapter = new();
        private static readonly Vector3Adapter Vector3Adapter = new();
        private static readonly Vector4Adapter Vector4Adapter = new();
        
        public static KalmanFilter<T> Create<T>(
            IKalmanValueAdapter<T> adapter,
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) where T : struct =>
        KalmanFilter<T>.Build(adapter, q, r, p);

        public static KalmanFilter<float> CreateFloatFilter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float>.Build(FloatAdapter, q, r, p);

        public static KalmanFilter<Vector2> CreateVector2Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector2>.Build(Vector2Adapter, q, r, p);

        public static IKalmanFilter<Vector3> CreateVector3Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector3>.Build(Vector3Adapter, q, r, p);

        public static IKalmanFilter<Vector4> CreateVector4Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<Vector4>.Build(Vector4Adapter, q, r, p);
        
#if UNITY_2022_3_OR_NEWER
        private static readonly Float2Adapter Float2Adapter = new();
        private static readonly Float3Adapter Float3Adapter = new();
        private static readonly Float4Adapter Float4Adapter = new();

        public static KalmanFilter<float2> CreateFloat2Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float2>.Build(Float2Adapter, q, r, p);

        public static KalmanFilter<float3> CreateFloat3Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float3>.Build(Float3Adapter, q, r, p);

        public static KalmanFilter<float4> CreateFloat4Filter(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f
        ) => KalmanFilter<float4>.Build(Float4Adapter, q, r, p);
#endif
    }
}