using System.Collections.Generic;
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;

namespace Aceland.KalmanFilter.Core
{
    [BurstCompile]
    public abstract class KalmanFilterBase<T> : IKalmanFilter<T> where T : struct
    {
        protected KalmanFilterBase(float q, float r, float p)
        {
            Q = q;
            R = r;
            P = p;
            K = 0f;
        }

        protected float Q;
        protected float R;
        protected float P;
        protected float K;
        protected T X;

        public (T x, float p, float k) GetCurrentValues() => (X, P, K);

        public void SetValues(T x, float p, float k)
        {
            X = x;
            P = p;
            K = k;
        }

        public virtual T Update(T measurement, float? newQ = null, float? newR = null) => default;

        public virtual T Update(List<T> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null) => default;

        public virtual void Reset()
        {
            P = 1f;
            K = 0f;
            X = default;
        }
    }
}