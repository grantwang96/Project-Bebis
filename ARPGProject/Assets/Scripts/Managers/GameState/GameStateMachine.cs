using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston {
    public class GameStateMachine : MonoBehaviour {

        public static GameStateMachine Instance { get; private set; }

        public AbstractState CurrentState { get; private set; }

        [SerializeField] private AbstractState _bootState;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            // enter the first state of the game
            ChangeState(_bootState);
        }

        public bool HandleTransition(string id) {
            // ensure we have a current state first
            if (CurrentState == null) {
                Debug.LogError($"[{nameof(GameStateMachine)}]: Current State is null!");
                return false;
            }
            // check if there is a matching transition with this id
            if (!CurrentState.TryGetGameStateFromTransitionId(id, out AbstractState stateToTransitionTo)) {
                Debug.LogWarning($"[{nameof(GameStateMachine)}]: Could not retrieve game state from id \"{id}\"!");
                return false;
            }
            ChangeState(stateToTransitionTo);
            // if we have a matching transition, change state to the corresponding game state
            return true;
        }

        private void ChangeState(AbstractState nextState) {
            CurrentState?.TryExit(nextState);
            CurrentState = nextState;
            CurrentState?.TryEnter();
        }
    }
}
