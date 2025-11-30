using UnityEngine;

namespace Aceland.KalmanFilter.Core
{
    internal static class KalmanValueAdapters
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RegisterDefaultAdapters()
        {
            KalmanValueAdapterCache<float>.Register(new FloatAdapter());
            KalmanValueAdapterCache<double>.Register(new DoubleAdapter());
#if UNITY_2021_3_OR_NEWER
            KalmanValueAdapterCache<Vector2>.Register(new Vector2Adapter());
            KalmanValueAdapterCache<Vector3>.Register(new Vector3Adapter());
            KalmanValueAdapterCache<Vector4>.Register(new Vector4Adapter());
#endif
        }

        public static void Register<T>(IKalmanValueAdapter<T> adapter) where T : unmanaged
        {
            KalmanValueAdapterCache<T>.Register(adapter);
        }

        #region Concrete adapters
        private sealed class FloatAdapter : IKalmanValueAdapter<float>
        {
            public float Zero => 0f;
            public float Add(float left, float right) => left + right;
            public float Subtract(float left, float right) => left - right;
            public float Scale(float value, float scalar) => value * scalar;
        }

        private sealed class DoubleAdapter : IKalmanValueAdapter<double>
        {
            public double Zero => 0d;
            public double Add(double left, double right) => left + right;
            public double Subtract(double left, double right) => left - right;
            public double Scale(double value, float scalar) => value * scalar;
        }

#if UNITY_2021_3_OR_NEWER
        private sealed class Vector2Adapter : IKalmanValueAdapter<Vector2>
        {
            public Vector2 Zero => Vector2.zero;
            public Vector2 Add(Vector2 left, Vector2 right) => left + right;
            public Vector2 Subtract(Vector2 left, Vector2 right) => left - right;
            public Vector2 Scale(Vector2 value, float scalar) => value * scalar;
        }

        private sealed class Vector3Adapter : IKalmanValueAdapter<Vector3>
        {
            public Vector3 Zero => Vector3.zero;
            public Vector3 Add(Vector3 left, Vector3 right) => left + right;
            public Vector3 Subtract(Vector3 left, Vector3 right) => left - right;
            public Vector3 Scale(Vector3 value, float scalar) => value * scalar;
        }

        private sealed class Vector4Adapter : IKalmanValueAdapter<Vector4>
        {
            public Vector4 Zero => Vector4.zero;
            public Vector4 Add(Vector4 left, Vector4 right) => left + right;
            public Vector4 Subtract(Vector4 left, Vector4 right) => left - right;
            public Vector4 Scale(Vector4 value, float scalar) => value * scalar;
        }
#endif
        #endregion
    }
}