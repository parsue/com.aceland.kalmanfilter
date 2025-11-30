using Aceland.KalmanFilter.Core;

namespace Aceland.KalmanFilter
{
    public static class KalmanFilter
    {
        public static IKalmanFilter<T> Build<T>(
            float q = 1e-6f,
            float r = 1e-3f,
            float p = 1f,
            IKalmanValueAdapter<T> adapter = null
        ) where T : unmanaged =>
        KalmanFilter<T>.Build(q, r, p, adapter);
        
        public static void RegisterAdapter<T>(IKalmanValueAdapter<T> adapter) where T : unmanaged =>
            KalmanValueAdapters.Register(adapter);
    }
}