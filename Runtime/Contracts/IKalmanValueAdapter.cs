namespace Aceland.KalmanFilter.Contracts
{
    /// <summary>
    /// Provides the arithmetic operations required by the Kalman filter for a given value type.
    /// Implementations must be blittable <c>struct</c>s so the filter stays fully Burst / Jobs / ECS compatible.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type (float, float2, Vector3, ...).</typeparam>
    public interface IKalmanValueAdapter<T> where T : unmanaged
    {
        T Zero { get; }
        T Add(T left, T right);
        T Subtract(T left, T right);
        T Scale(T value, float scalar);
    }
}
