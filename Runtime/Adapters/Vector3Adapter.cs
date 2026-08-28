using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    internal struct Vector3Adapter : IKalmanValueAdapter<Vector3>
    {
        public Vector3 Zero => Vector3.zero;
        public Vector3 Add(Vector3 left, Vector3 right) => left + right;
        public Vector3 Subtract(Vector3 left, Vector3 right) => left - right;
        public Vector3 Scale(Vector3 value, float scalar) => value * scalar;
    }
}