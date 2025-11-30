namespace Aceland.KalmanFilter
{
    public interface IKalmanValueAdapter<T> where T : unmanaged
    {
        T Zero { get; }
        T Add(T left, T right);
        T Subtract(T left, T right);
        T Scale(T value, float scalar);
    }
}