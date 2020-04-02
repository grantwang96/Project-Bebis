using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerActionController : PlayerCharacterComponent, IActionController {

        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;

        private IPlayerGameplayActionSet _normalGameplaySet;
        private IPlayerGameplayActionSet _skillMode1GameplaySet;
        private IPlayerGameplayActionSet _currentActionSet;

        public PlayerActionController(PlayerCharacter character) : base(character) {
            SubscribeToEvents();
        }

        private void SubscribeToEvents() {
            _character.OnStart += OnStart;
            _character.OnActionStatusUpdated += OnCharacterAnimationStatusSent;

            InputController.Instance.OnBtn1Pressed += OnBtn1Pressed;
            InputController.Instance.OnBtn1Held += OnBtn1Held;
            InputController.Instance.OnBtn1Released += OnBtn1Released;
        }

        private void UnsubscribeToEvents() {
            _character.OnStart -= OnStart;
            _character.ActionController.OnActionStatusUpdated -= OnCharacterAnimationStatusSent;

            InputController.Instance.OnBtn1Pressed -= OnBtn1Pressed;
            InputController.Instance.OnBtn1Held -= OnBtn1Held;
            InputController.Instance.OnBtn1Released -= OnBtn1Released;
        }

        private void OnStart() {
            SetGameplayActionSets();
        }

        private void SetGameplayActionSets() {
            _normalGameplaySet = new PlayerGameplayActionSet_NormalMode(_character, _character.HackConfig.NormalAttack, _character.HackConfig.SecondaryAttack);
            _currentActionSet = _normalGameplaySet;
        }

        private void OnCharacterAnimationStatusSent(ActionStatus status) {
            if(CurrentState == null) {
                return;
            }
            CurrentState.Status = status;
            OnActionStatusUpdated?.Invoke(status);
            if (CurrentState.Status.HasFlag(ActionStatus.Completed)) {
                CurrentState.Clear();
                CurrentState = null;
            }
        }

        private void OnBtn1Pressed() {
            if(_currentActionSet.A_BtnSkill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.A_BtnSkill.Initiate(_character, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Held() {
            if (_currentActionSet.A_BtnSkill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.A_BtnSkill.Hold(_character, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Released() {
            if (_currentActionSet.A_BtnSkill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.A_BtnSkill.Release(_character, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }
        
        private void PerformActionSuccess(ICharacterActionState state) {
            CurrentState?.Clear();
            CurrentState = state;
            _character.AnimationController.UpdateAnimationState(CurrentState.AnimationData);
            OnPerformActionSuccess?.Invoke();
        }
    }
}
