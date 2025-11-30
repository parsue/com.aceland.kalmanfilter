using System;

namespace Aceland.KalmanFilter.Core
{
    internal static class KalmanValueAdapterCache<T> where T : unmanaged
    {
        private static IKalmanValueAdapter<T> _adapter;

        internal static void Register(IKalmanValueAdapter<T> adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        internal static IKalmanValueAdapter<T> GetOrThrow()
        {
            if (_adapter == null)
            {
                throw new InvalidOperationException(
                    $"No Kalman value adapter registered for {typeof(T).FullName}. " +
                    "Register one via KalmanValueAdapters.RegisterDefaultAdapters() or your own adapter.");
            }
            return _adapter;
        }

        internal static bool IsRegistered => _adapter != null;
    }
}