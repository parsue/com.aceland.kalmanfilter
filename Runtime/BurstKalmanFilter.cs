using Unity.Burst;
using Unity.Mathematics;

namespace Aceland.KalmanFilter
{
    [BurstCompile]
    public struct BurstKalmanFilter<TValue, TAdapter>
        where TValue : unmanaged
        where TAdapter : struct, IKalmanValueAdapter<TValue>
    {
        private float _q;
        private float _r;
        private float _p;
        private float _k;
        private TValue _x;
        private TAdapter _ops;

        public BurstKalmanFilter(float q, float r, float p, TAdapter ops)
        {
            _q = q;
            _r = r;
            _p = p;
            _k = 0f;
            _x = ops.Zero;
            _ops = ops;
        }

        public TValue Update(TValue measurement, float? newQ = null, float? newR = null)
        {
            if (newQ.HasValue && math.abs(_q - newQ.Value) > math.EPSILON)
                _q = newQ.Value;
            if (newR.HasValue && math.abs(_r - newR.Value) > math.EPSILON)
                _r = newR.Value;

            var innovationCovariance = _p + _q + _r;
            if (innovationCovariance == 0f)
                return _x;

            _k = (_p + _q) / innovationCovariance;
            _p = _r * _k;

            var innovation = _ops.Subtract(measurement, _x);
            _x = _ops.Add(_x, _ops.Scale(innovation, _k));
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