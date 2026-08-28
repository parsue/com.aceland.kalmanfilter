namespace Aceland.KalmanFilter.Contracts
{
    /// <summary>
    /// Burst-friendly Kalman filter contract. The concrete implementation is a value type,
    /// so prefer using it directly (by ref) inside Jobs / Burst code. This interface exists
    /// only for managed convenience; accessing the filter through the interface boxes it once.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type (float, float2, Vector3, ...).</typeparam>
    public interface IKalmanFilter<T> where T : unmanaged
    {
        (T x, float p, float k) GetCurrentValues();
        void SetValues(T x, float p, float k);

        /// <summary>
        /// Feeds a single measurement and returns the filtered estimate.
        /// Pass <see cref="float.NaN"/> (the default) to keep the current noise values unchanged.
        /// </summary>
        T Update(T measurement, float newQ = float.NaN, float newR = float.NaN);

        void Reset();
    }
}
