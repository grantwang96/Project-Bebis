using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface INPCActionController : IActionController {

    }

    public class NPCActionController : CharacterComponent, INPCActionController {
        
        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;

        private void Start() {
            _character.AnimationController.OnActionStatusUpdated += OnCharacterAnimationStatusSent;
        }

        public bool PerformAction(CharacterActionData data, CharacterActionContext context) {
            bool success = false;
            switch (context) {
                case CharacterActionContext.Initiate:
                    success = InitiateAction(data, context);
                    break;
                case CharacterActionContext.Hold:
                    success = HoldAction(data, context);
                    break;
                case CharacterActionContext.Release:
                    success = ReleaseAction(data, context);
                    break;
            }
            return success;
        }

        // force clear the action from an external source (ex. getting hit)
        public void ClearCurrentActionState() {
            OnActionStatusUpdated?.Invoke(ActionStatus.Completed);
            CurrentState?.Clear();
            CurrentState = null;
        }

        private bool InitiateAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Initiate(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
            return response.Success;
        }

        private bool HoldAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Hold(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
            return response.Success;
        }

        private bool ReleaseAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Release(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
            return response.Success;
        }

        private void PerformActionSuccess(ICharacterActionState state) {
            CurrentState?.Clear();
            if (state != null) {
                CurrentState = state;
                _character.AnimationController.UpdateAnimationState(CurrentState?.AnimationData);
                OnPerformActionSuccess?.Invoke();
                OnCharacterAnimationStatusSent(CurrentState.Status);
            }
        }

        private void OnCharacterAnimationStatusSent(ActionStatus status) {
            if (CurrentState == null) {
                return;
            }
            CurrentState.Status = status;
            OnActionStatusUpdated?.Invoke(status);
            if (status.HasFlag(ActionStatus.Completed)) {
                CurrentState?.Clear();
                CurrentState = null;
            }
        }
    }
}
