using System.Collections.Generic;

namespace Aceland.KalmanFilter.Contracts
{
    public interface IKalmanFilter<T> where T : struct
    {
        (T x, float p, float k) GetCurrentValues();
        void SetValues(T x, float p, float k);
        
        T Update(T measurement, float? newQ = null, float? newR = null);
        T Update(List<T> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null);
        void Reset();
    }
}