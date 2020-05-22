using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerActionController : MonoBehaviour, IActionController {

        [SerializeField] private PlayerCharacter _playerCharacter;

        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;

        private IPlayerGameplayActionSet _normalGameplaySet;
        private IPlayerGameplayActionSet _skillMode1GameplaySet;
        private IPlayerGameplayActionSet _currentActionSet;

        private void Start() {
            SetGameplayActionSets();
            SubscribeToInputController();
            SubscribeToAnimationController();
        }

        private void SetGameplayActionSets() {
            HackPlayerConfig hackConfig = _playerCharacter.HackConfig;
            _normalGameplaySet = new PlayerGameplayActionSet_NormalMode(
                _playerCharacter,
                hackConfig.JumpAction,
                hackConfig.NormalAttack,
                hackConfig.SecondaryAttack);
            _currentActionSet = _normalGameplaySet;
        }

        private void SubscribeToInputController() {
            InputController.Instance.OnBtn1Pressed += OnBtn1Pressed;
            InputController.Instance.OnBtn1Held += OnBtn1Held;
            InputController.Instance.OnBtn1Released += OnBtn1Released;

            InputController.Instance.OnBtn2Pressed += OnBtn2Pressed;
            InputController.Instance.OnBtn2Held += OnBtn2Held;
            InputController.Instance.OnBtn2Released += OnBtn2Released;
        }

        private void SubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated += OnCharacterAnimationStatusSent;
        }

        private void UnsubscribeToInputController() {
            InputController.Instance.OnBtn1Pressed -= OnBtn1Pressed;
            InputController.Instance.OnBtn1Held -= OnBtn1Held;
            InputController.Instance.OnBtn1Released -= OnBtn1Released;

            InputController.Instance.OnBtn2Pressed -= OnBtn2Pressed;
            InputController.Instance.OnBtn2Held -= OnBtn2Held;
            InputController.Instance.OnBtn2Released -= OnBtn2Released;
        }

        private void UnsubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated -= OnCharacterAnimationStatusSent;
        }

        private void OnBtn1Pressed() {
            if(_currentActionSet.Btn1Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Held() {
            if (_currentActionSet.Btn1Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Released() {
            if (_currentActionSet.Btn1Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Pressed() {
            if(_currentActionSet.Btn2Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Held() {
            if (_currentActionSet.Btn2Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Released() {
            if (_currentActionSet.Btn2Skill == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }
        
        private void PerformActionSuccess(ICharacterActionState state) {
            CurrentState?.Clear();
            CurrentState = state;
            _playerCharacter.AnimationController.UpdateAnimationState(CurrentState.AnimationData);
            OnPerformActionSuccess?.Invoke();
            OnCharacterAnimationStatusSent(CurrentState.Status);
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
