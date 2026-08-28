using Aceland.KalmanFilter.Contracts;
using Unity.Burst;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    public readonly struct FloatAdapter : IKalmanValueAdapter<float>
    {
        public float Zero => 0f;
        public float Add(float left, float right) => left + right;
        public float Subtract(float left, float right) => left - right;
        public float Scale(float value, float scalar) => value * scalar;
    }
}
