#if UNITY_2022_3_OR_NEWER
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using Unity.Mathematics;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    internal struct Float3Adapter : IKalmanValueAdapter<float3>
    {
        public float3 Zero => float3.zero;
        public float3 Add(float3 left, float3 right) => left + right;
        public float3 Subtract(float3 left, float3 right) => left - right;
        public float3 Scale(float3 value, float scalar) => value * scalar;
    }
}
#endif