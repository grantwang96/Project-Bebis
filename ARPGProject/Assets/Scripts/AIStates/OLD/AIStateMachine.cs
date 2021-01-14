using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class AIStateMachine : MonoBehaviour {

        [SerializeField] private AIState _currentState;

        private Dictionary<string, AIState> _allAIStates = new Dictionary<string, AIState>();
        private Queue<AIState> _queuedAIStates = new Queue<AIState>();

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

        private void OnReadyToChangeState(AIState state) {
            _currentState.OnReadyToChangeState -= OnReadyToChangeState;
            AIState nextState = state;
            if(_queuedAIStates.Count > 0) {
                nextState = _queuedAIStates.Dequeue();
            }
            _currentState?.Exit(nextState);
            _currentState = nextState;
            _currentState.OnReadyToChangeState += OnReadyToChangeState;
            _currentState?.Enter();
        }

        public void RegisterAIState(AIState state) {
            if (_allAIStates.ContainsKey(state.Id)) {
                return;
            }
            _allAIStates.Add(state.Id, state);
        }

        public void AddStateToQueue(AIState state) {
            _queuedAIStates.Enqueue(state);
        }
    }
}
