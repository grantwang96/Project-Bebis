using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    private readonly List<TimerObject> _allTimerObjects = new List<TimerObject>();
    private readonly Dictionary<string, TimerObject> _timersById = new Dictionary<string, TimerObject>();

    private readonly List<TimerObject> _timersToRemove = new List<TimerObject>();

    private bool _active = true;

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllTimers();
    }

    private void LateUpdate() {
        RemoveCompletedTimers();
    }

    public void AddTimer(TimerObject timer) {
        _allTimerObjects.Add(timer);
        _timersById.Add(timer.Id, timer);

        timer.OnTimerCompleted += OnTimerCompleted;
    }

    public bool TryGetTimer(string id, out TimerObject timer) {
        return _timersById.TryGetValue(id, out timer);
    }

    public void RemoveTimer(string id) {
        if(!_timersById.TryGetValue(id, out TimerObject timer)) {
            return;
        }
        timer.OnTimerCompleted -= OnTimerCompleted;
        _timersById.Remove(id);
        _allTimerObjects.Remove(timer);
    }

    private void UpdateAllTimers() {
        if (!_active) {
            return;
        }
        for(int i = 0; i < _allTimerObjects.Count; i++) {
            _allTimerObjects[i].Increment();
        }
    }

    private void RemoveCompletedTimers() {
        if (!_active) {
            return;
        }
        for (int i = 0; i < _timersToRemove.Count; i++) {
            RemoveTimer(_timersToRemove[i].Id);
        }
        _timersToRemove.Clear();
    }

    private void OnTimerCompleted(TimerObject timer) {
        _timersToRemove.Add(timer);
    }

    private void OnGamePaused(bool paused) {
        _active = !paused;
    }
}

public abstract class TimerObject {

    public string Id { get; private set; }
    public float TargetTime { get; private set; }
    public float CurrentTime { get; private set; }
    public bool Completed { get; private set; }

    public event Action<TimerObject> OnTimerCompleted;

    public TimerObject(string id, float targetTime) {
        Id = id;
        CurrentTime = 0f;
        TargetTime = targetTime;
        Completed = false;
    }

    public void Increment() {
        CurrentTime += Time.deltaTime;
        if(CurrentTime >= TargetTime) {
            DoTimerAction();
            Completed = true;
        }
    }

    protected virtual void DoTimerAction() {
        OnTimerCompleted?.Invoke(this);
    }
}

public class SimpleActionTimer : TimerObject {

    private Action _callback;

    public SimpleActionTimer(string id, float duration, Action action) : base(id, duration) {
        _callback = action;
    }

    protected override void DoTimerAction() {
        _callback?.Invoke();
        base.DoTimerAction();
    }
}
