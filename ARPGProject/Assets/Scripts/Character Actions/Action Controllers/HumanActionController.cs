using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HumanActionController : IActionControllerV2
    {
        public ICharacterActionStateV2 CurrentState { get; private set; }

        public event Action<ICharacterActionStateV2> OnCurrentStateUpdated;
        public event Action<ICharacterActionStateV2> OnActionStateUpdated;

        private ICharacterV2 _character;
        private readonly Dictionary<string, ICharacterActionStateV2> _actionStates = new Dictionary<string, ICharacterActionStateV2>();

        public void Initialize(ICharacterV2 character) {
            _character = character;
            SubscribeToEvents();
        }

        private void SubscribeToEvents() {
            _character.UnitController.OnActionAttempted += OnActionAttempted;
            _character.UnitController.OnRegisteredCharacterActionsUpdated += OnRegisteredCharacterActionsUpdated;
        }

        private void OnRegisteredCharacterActionsUpdated(IReadOnlyList<ICharacterActionDataV2> datas) {
            // clean up the current set of actions
            foreach(var kvp in _actionStates) {
                kvp.Value.OnActionStatusUpdated -= OnActionStatusUpdated;
                kvp.Value.Clear();
            }
            _actionStates.Clear();
            // generate a new set of actions
            for(int i = 0; i < datas.Count; i++) {
                ICharacterActionStateV2 newState = datas[i].GenerateCharacterActionState(_character);
                newState.OnActionStatusUpdated += OnActionStatusUpdated;
                _actionStates.Add(datas[i].Id, newState);
                
            }
        }

        private void OnActionAttempted(ICharacterActionDataV2 data, CharacterActionContext context) {
            // try to retrieve the action state from the registry
            if(!_actionStates.TryGetValue(data.Id, out ICharacterActionStateV2 state)) {
                return;
            }
            // attempt to perform the action
            bool success = state.TryPerformAction(context);
            // if successful, set the action as the current action state and notify all listeners
            if (success) {
                CurrentState = state;
                OnCurrentStateUpdated?.Invoke(state);
            }
        }

        private void OnActionStatusUpdated(ICharacterActionStateV2 state, ActionStatus status) {
            OnActionStateUpdated?.Invoke(state);
            if (status == ActionStatus.Completed) {
                if(state == CurrentState) {
                    CurrentState = null;
                    OnCurrentStateUpdated?.Invoke(CurrentState);
                }
                state.Clear();
            }
        }
    }
}
