using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonoBehaviourMaster : MonoBehaviour
{
    public static MonoBehaviourMaster Instance { get; private set; }

    public event Action OnAwake;
    public event Action OnStart;
    public event Action OnUpdate;
    public event Action OnFixedUpdate;

    private void Awake() {
        Instance = this;
        OnAwake?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate?.Invoke();
    }

    private void FixedUpdate() {
        OnFixedUpdate?.Invoke();
    }
}
