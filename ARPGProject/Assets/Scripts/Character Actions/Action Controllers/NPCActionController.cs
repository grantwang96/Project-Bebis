using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface INPCActionController : IActionController {

    }

    public class NPCActionController : CharacterComponent, INPCActionController {

        public ICharacterActionState CurrentState { get; private set; }

        public event Action OnCurrentActionUpdated;

        [SerializeField] private NPCCharacter _npcCharacter;

        private IActionControllerInfoProvider _actionControllerInfoProvider;
        private readonly Dictionary<ICharacterActionData, ICharacterActionState> _currentActionStates = new Dictionary<ICharacterActionData, ICharacterActionState>();

        // initialization
        public void Initialize(IActionControllerInfoProvider infoProvider) {

            if (_actionControllerInfoProvider != null) {
                _actionControllerInfoProvider.OnActionAttempted -= OnActionAttempted;
            }

            _actionControllerInfoProvider = infoProvider;
            _actionControllerInfoProvider.OnActionAttempted += OnActionAttempted;
        }

        // when an action is attempted
        private void OnActionAttempted(ICharacterActionData data, CharacterActionContext context) {
            // check if we have a state registered for this data
            bool containsState = _currentActionStates.TryGetValue(data, out ICharacterActionState characterActionState);
            CharacterActionResponse response = new CharacterActionResponse();
            // process the interaction with the action data
            switch (context) {
                case CharacterActionContext.Initiate:
                    response = data.Initiate(_npcCharacter, characterActionState, context);
                    break;
                case CharacterActionContext.Hold:
                    response = data.Hold(_npcCharacter, characterActionState, context);
                    break;
                case CharacterActionContext.Release:
                    response = data.Release(_npcCharacter, characterActionState, context);
                    break;
                default:
                    Debug.Log($"[{name}]: Invalid interaction {context}");
                    break;
            }
            // if we didn't successfully perform the action
            if (!response.Success) {
                return;
            }
            // if we didn't have a state for this data
            if (!containsState) {
                _currentActionStates.Add(data, response.State);
                response.State.OnActionStatusUpdated += OnActionStatusUpdated;
            }
            // if this is a different action state
            if (response.State != CurrentState && response.State.Status != ActionStatus.Completed) {
                CurrentState = response.State;
                OnCurrentActionUpdated?.Invoke();
            }
        }

        // when an action has been completed, remove it from the registry
        private void OnActionStatusUpdated(ICharacterActionState characterActionState, ActionStatus status) {
            switch (status) {
                case ActionStatus.Completed:
                    OnActionCompleted(characterActionState);
                    break;
            }
        }

        private void OnActionCompleted(ICharacterActionState characterActionState) {
            characterActionState.OnActionStatusUpdated -= OnActionStatusUpdated;
            characterActionState.Clear();
            _currentActionStates.Remove(characterActionState.Data);
            if (CurrentState == characterActionState) {
                CurrentState = null;
                OnCurrentActionUpdated?.Invoke();
            }
        }
    }
}
