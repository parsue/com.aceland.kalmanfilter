using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using Unity.Collections;

namespace Aceland.KalmanFilter.Core
{
    /// <summary>
    /// A blittable, Burst-compatible scalar Kalman filter.
    /// The value math is provided by <typeparamref name="TAdapter"/>, a value-type strategy that is
    /// resolved at compile time (no interface dispatch, no boxing), so the whole struct can live inside
    /// Burst-compiled Jobs, ECS <c>ISystem</c>s, or plain managed code without any GC allocation.
    /// </summary>
    /// <typeparam name="T">The unmanaged value type being filtered (float, float2, Vector3, ...).</typeparam>
    /// <typeparam name="TAdapter">The value-type adapter that implements the arithmetic for <typeparamref name="T"/>.</typeparam>
    [BurstCompile]
    public struct KalmanFilter<T, TAdapter> : IKalmanFilter<T>
        where T : unmanaged
        where TAdapter : unmanaged, IKalmanValueAdapter<T>
    {
        private TAdapter _ops;

        private float _q;
        private float _r;
        private float _p;
        private float _k;
        private T _x;

        internal static KalmanFilter<T, TAdapter> Build(
            TAdapter adapter,
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f)
        {
            return new KalmanFilter<T, TAdapter>
            {
                _ops = adapter,
                _q = q,
                _r = r,
                _p = p,
                _k = 0f,
                _x = adapter.Zero,
            };
        }

        public (T x, float p, float k) GetCurrentValues() => (_x, _p, _k);

        public void SetValues(T x, float p, float k)
        {
            _x = x;
            _p = p;
            _k = k;
        }

        /// <summary>
        /// Feeds a single measurement and returns the filtered estimate.
        /// Pass <see cref="float.NaN"/> (the default) to keep the current noise values unchanged.
        /// </summary>
        public T Update(T measurement, float newQ = float.NaN, float newR = float.NaN)
        {
            if (!float.IsNaN(newQ)) _q = newQ;
            if (!float.IsNaN(newR)) _r = newR;

            var innovationCovariance = _p + _q + _r;
            if (innovationCovariance == 0f)
                return _x;

            _k = (_p + _q) / innovationCovariance;
            _p = _r * _k;

            var innovation = _ops.Subtract(measurement, _x);
            var correction = _ops.Scale(innovation, _k);
            _x = _ops.Add(_x, correction);

            return _x;
        }

        /// <summary>
        /// Feeds a batch of measurements from a <see cref="NativeArray{T}"/> (Job / Burst friendly) and
        /// returns the final filtered estimate. Measurements are processed from index 0 upward unless
        /// <paramref name="areMeasurementsNewestFirst"/> is <c>true</c>.
        /// </summary>
        public T Update(NativeArray<T> measurements, bool areMeasurementsNewestFirst = false,
            float newQ = float.NaN, float newR = float.NaN)
        {
            if (measurements.Length == 0)
                return _x;

            if (areMeasurementsNewestFirst)
            {
                for (var i = measurements.Length - 1; i >= 0; --i)
                    Update(measurements[i], newQ, newR);
            }
            else
            {
                for (var i = 0; i < measurements.Length; ++i)
                    Update(measurements[i], newQ, newR);
            }

            return _x;
        }

        public void Reset()
        {
            _p = 1f;
            _k = 0f;
            _x = _ops.Zero;
        }
    }
}
