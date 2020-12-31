using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HumanActionController : IActionControllerV2
    {
        public ICharacterActionStateV2 CurrentState { get; }

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
            bool hasAction = _actionStates.TryGetValue(data.Id, out ICharacterActionStateV2 actionState);
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
            if (!response.Success) {
                return;
            }
            if (!hasAction) {
                _actionStates.Add(data.Id, response.State);
                response.State.OnActionStatusUpdated += OnActionStatusUpdated;
            }
        }

        private void OnActionStatusUpdated(ICharacterActionStateV2 state, ActionStatus status) {
            if(status == ActionStatus.Completed) {
                state.OnActionStatusUpdated -= OnActionStatusUpdated;
                state.Clear();
                _actionStates.Remove(state.Data.Id);
            }
        }
    }
}
