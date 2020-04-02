using System;

public class GameEvent {
    public event Action GameEventAction;

    public void Broadcast() {
        GameEventAction?.Invoke();
    }

    public void Subscribe(Action action) {
        GameEventAction += action;
    }

    public void Unsubscribe(Action action) {
        GameEventAction -= action;
    }

    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}

public class GameEvent<T> {
    public event Action<T> GameEventAction;

    public void Broadcast(T arg0) {
        GameEventAction?.Invoke(arg0);
    }

    public void Subscribe(Action<T> action) {
        GameEventAction += action;
    }

    public void Unsubscribe(Action<T> action) {
        GameEventAction -= action;
    }

    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}
