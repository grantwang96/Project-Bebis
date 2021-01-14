using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class AIStateMachineV2 : ICharacterComponent
    {
        private ICharacterV2 _character;
        private string _startingState;
        private readonly Dictionary<string, IAIState> _allStates = new Dictionary<string, IAIState>();
        private IAIState _currentState;

        public void Initialize(ICharacterV2 character) {
            _character = character;
        }

        public void GenerateAIStateTree(AIStateData aiStateData, string startingState) {
            AIStateTreeBuilder.Build(_character, this, aiStateData);
            _startingState = startingState;
        }

        public void StartStateMachine() {
            if (!TryGetAIState(_startingState, out _currentState)) {
                Debug.LogError($"[{_character.GameObject.name}]: Could not retrieve starting AI State with id \'{_startingState}\'");
                return;
            }
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
