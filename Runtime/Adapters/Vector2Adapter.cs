using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

namespace Aceland.KalmanFilter.Adapters
{
    [BurstCompile]
    internal struct Vector2Adapter : IKalmanValueAdapter<Vector2>
    {
        public Vector2 Zero => Vector2.zero;
        public Vector2 Add(Vector2 left, Vector2 right) => left + right;
        public Vector2 Subtract(Vector2 left, Vector2 right) => left - right;
        public Vector2 Scale(Vector2 value, float scalar) => value * scalar;
    }
}