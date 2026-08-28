namespace Aceland.KalmanFilter.Contracts
{
    public interface IKalmanValueAdapter<T> where T : struct
    {
        T Zero { get; }
        T Add(T left, T right);
        T Subtract(T left, T right);
        T Scale(T value, float scalar);
    }
}