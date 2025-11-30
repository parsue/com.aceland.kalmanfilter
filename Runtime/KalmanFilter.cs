using Aceland.KalmanFilter.Builders;

namespace Aceland.KalmanFilter
{
    public static class KalmanFilter
    {
        public static IKalmanFilterBuilder<T> Builder<T>() where T : unmanaged =>
            new KalmanFilterBuilder<T>();
    }
}