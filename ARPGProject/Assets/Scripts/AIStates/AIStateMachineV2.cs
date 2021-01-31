using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class AIStateMachineV2 : MonoBehaviour, ICharacterComponent
    {
        [SerializeField] private AIStateV2 _startingState;
        [SerializeField] private List<AIStateV2> _aiStatesList = new List<AIStateV2>();

        private ICharacterV2 _character;
        private readonly Dictionary<string, IAIState> _allStates = new Dictionary<string, IAIState>();
        private IAIState _currentState;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            for(int i = 0; i < _aiStatesList.Count; i++) {
                _aiStatesList[i].Initialize(_character);
            }
            for(int i = 0; i < _aiStatesList.Count; i++) {
                _aiStatesList[i].Setup();
            }
        }

        public void StartStateMachine() {
            _currentState = _startingState;
            _currentState.OnReadyToTransition += OnReadyToChangeState;
            _currentState.TryEnter();
        }

        public void RegisterAIState(IAIState aiState) {
            // ensure there are no repeat states
            if (_allStates.ContainsKey(aiState.Id)) {
                return;
            }
            _allStates.Add(aiState.Id, aiState);
        }

        public bool TryGetAIState(string id, out IAIState aiState) {
            return _allStates.TryGetValue(id, out aiState);
        }

        private void OnReadyToChangeState(IAIState nextState) {
            _currentState.OnReadyToTransition -= OnReadyToChangeState;
            _currentState?.TryExit(nextState);
            _currentState = nextState;
            _currentState.OnReadyToTransition += OnReadyToChangeState;
            _currentState?.TryEnter();
        }
    }
}
