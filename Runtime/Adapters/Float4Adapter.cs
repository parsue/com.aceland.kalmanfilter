#if UNITY_2022_3_OR_NEWER
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using Unity.Mathematics;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    public readonly struct Float4Adapter : IKalmanValueAdapter<float4>
    {
        public float4 Zero => float4.zero;
        public float4 Add(float4 left, float4 right) => left + right;
        public float4 Subtract(float4 left, float4 right) => left - right;
        public float4 Scale(float4 value, float scalar) => value * scalar;
    }
}
#endif
