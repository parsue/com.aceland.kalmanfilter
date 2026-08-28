using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    internal struct Vector4Adapter : IKalmanValueAdapter<Vector4>
    {
        public Vector4 Zero => Vector4.zero;
        public Vector4 Add(Vector4 left, Vector4 right) => left + right;
        public Vector4 Subtract(Vector4 left, Vector4 right) => left - right;
        public Vector4 Scale(Vector4 value, float scalar) => value * scalar;
    }
}