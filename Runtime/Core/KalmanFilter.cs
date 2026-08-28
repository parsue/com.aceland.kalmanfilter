using System;
using System.Collections.Generic;
using Aceland.KalmanFilter.Contracts;
using Unity.Burst;
using UnityEngine;

namespace Aceland.KalmanFilter.Core
{
    [BurstCompile]
    public sealed class KalmanFilter<T> : KalmanFilterBase<T> where T : struct
    {
        private readonly IKalmanValueAdapter<T> _ops;

        internal static KalmanFilter<T> Build(
            IKalmanValueAdapter<T> adapter,
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f)
        {
            var ops = adapter;
            return new KalmanFilter<T>(q, r, p, ops);
        }

        private KalmanFilter(float q, float r, float p, IKalmanValueAdapter<T> ops)
            : base(q, r, p)
        {
            _ops = ops;
            X = ops.Zero;
        }

        public override T Update(T measurement, float? newQ = null, float? newR = null)
        {
            if (newQ.HasValue && !Mathf.Approximately(Q, newQ.Value)) Q = newQ.Value;
            if (newR.HasValue && !Mathf.Approximately(R, newR.Value)) R = newR.Value;

            var innovationCovariance = P + Q + R;
            if (innovationCovariance == 0f)
                return X;

            K = (P + Q) / innovationCovariance;
            P = R * K;

            var innovation = _ops.Subtract(measurement, X);
            var correction = _ops.Scale(innovation, K);
            X = _ops.Add(X, correction);

            return X;
        }

        public override T Update(List<T> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null)
        {
            if (measurements == null || measurements.Count == 0)
                throw new ArgumentException("Measurements list is null or empty.", nameof(measurements));

            var result = X;
            if (areMeasurementsNewestFirst)
            {
                for (var i = measurements.Count - 1; i >= 0; --i)
                    result = Update(measurements[i], newQ, newR);
            }
            else
            {
                for (var i = 0; i < measurements.Count; ++i)
                    result = Update(measurements[i], newQ, newR);
            }
            return result;
        }

        public override void Reset()
        {
            base.Reset();
            X = _ops.Zero;
        }
    }
}