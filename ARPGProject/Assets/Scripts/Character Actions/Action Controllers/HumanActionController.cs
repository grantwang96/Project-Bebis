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
        }

        private void OnActionAttempted(ICharacterActionDataV2 data, CharacterActionContext context) {
            // check if the requisite action state already exists
            bool hasAction = _actionStates.TryGetValue(data.Id, out ICharacterActionStateV2 actionState);
            // attempt to perform the action
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
            switch(context) {
                case CharacterActionContext.Initiate:
                    response = data.Initiate(_character, actionState, context);
                    break;
                case CharacterActionContext.Hold:
                    response = data.Hold(_character, actionState, context);
                    break;
                case CharacterActionContext.Release:
                    response = data.Release(_character, actionState, context);
                    break;
                default:
                    break;
            }
            // if the action is denied, return and do nothing
            if (!response.Success) {
                return;
            }
            // if this is a new action state, register it
            if (!hasAction) {
                _actionStates.Add(data.Id, response.State);
                response.State.OnActionStatusUpdated += OnActionStatusUpdated;
            }
            // update the current action state and notify all listeners
            CurrentState = response.State;
            OnCurrentStateUpdated?.Invoke(response.State);
        }

        private void OnActionStatusUpdated(ICharacterActionStateV2 state, ActionStatus status) {
            OnActionStateUpdated?.Invoke(state);
            if (status == ActionStatus.Completed) {
                state.OnActionStatusUpdated -= OnActionStatusUpdated;
                state.Clear();
                _actionStates.Remove(state.Data.Id);
                if(state == CurrentState) {
                    CurrentState = null;
                    OnCurrentStateUpdated?.Invoke(CurrentState);
                }
            }
        }
    }
}
