namespace ProJob.Observer;

public abstract class Observable<T>
{
    private readonly List<IGameObserver<T>> _observers = [];

    public void Subscribe(IGameObserver<T> observer) => _observers.Add(observer);

    public void Unsubscribe(IGameObserver<T> observer) => _observers.Remove(observer);

    protected void Notify(T notification)
    {
        foreach (var observer in _observers.ToList())
            observer.OnNotify(notification);
    }
}
