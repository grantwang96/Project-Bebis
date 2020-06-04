using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface INPCActionController : IActionController {
        void PerformAction(CharacterActionData data, CharacterActionContext context);
    }

    public class NPCActionController : CharacterComponent, INPCActionController {
        
        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;
        
        public void PerformAction(CharacterActionData data, CharacterActionContext context) {
            switch (context) {
                case CharacterActionContext.Initiate:
                    InitiateAction(data, context);
                    break;
                case CharacterActionContext.Hold:
                    HoldAction(data, context);
                    break;
                case CharacterActionContext.Release:
                    ReleaseAction(data, context);
                    break;
            }
        }

        private void InitiateAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Initiate(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void HoldAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Hold(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void ReleaseAction(CharacterActionData data, CharacterActionContext context) {
            CharacterActionResponse response = data.Release(_character, CurrentState, context);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
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
            if (CurrentState.Status.HasFlag(ActionStatus.Completed)) {
                CurrentState.Clear();
                CurrentState = null;
            }
        }
    }
}
