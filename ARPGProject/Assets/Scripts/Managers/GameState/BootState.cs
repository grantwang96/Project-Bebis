using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public class BootState : MonoBehaviour
    {
        [SerializeField] private float _tempWaitTime;
        [SerializeField] private string _transitionId;
        [SerializeField] private GameState _gameState;
        [SerializeField] private GameStateMachine _gameStateMachine;

        private void Awake() {
            _gameState.OnStateEntered += OnEnter;
        }

        private void OnEnter(bool success) {
            if (!success) {
                return;
            }
            TimerManager.Instance.AddTimer(new SimpleActionTimer(nameof(BootState), _tempWaitTime, OnBootStateComplete));
        }

        private void OnBootStateComplete() {
            _gameStateMachine.HandleTransition(_transitionId);
        }
    }
}
