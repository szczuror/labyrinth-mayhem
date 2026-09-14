namespace ProJob.Observer;

public interface IGameObserver<T>
{
    void OnNotify(T notification);
}
