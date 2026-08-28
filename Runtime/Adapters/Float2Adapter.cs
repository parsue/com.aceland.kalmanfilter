#if UNITY_2022_3_OR_NEWER
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using Unity.Mathematics;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    internal struct Float2Adapter : IKalmanValueAdapter<float2>
    {
        public float2 Zero => float2.zero;
        public float2 Add(float2 left, float2 right) => left + right;
        public float2 Subtract(float2 left, float2 right) => left - right;
        public float2 Scale(float2 value, float scalar) => value * scalar;
    }
}
#endif