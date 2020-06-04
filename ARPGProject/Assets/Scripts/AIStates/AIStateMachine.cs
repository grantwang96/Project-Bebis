using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class AIStateMachine : MonoBehaviour {

        [SerializeField] private AIState _currentState;

        // Start is called before the first frame update
        void Start() {
            _currentState.OnReadyToChangeState += OnReadyToChangeState;
            _currentState?.Enter();
        }

        // Update is called once per frame
        void Update() {
            if(_currentState != null) {
                _currentState.Execute();
            }
        }

        private void OnReadyToChangeState(AIState nextState) {
            _currentState.OnReadyToChangeState -= OnReadyToChangeState;
            _currentState?.Exit(nextState);
            _currentState = nextState;
            _currentState.OnReadyToChangeState += OnReadyToChangeState;
            _currentState?.Enter();
        }
    }
}
