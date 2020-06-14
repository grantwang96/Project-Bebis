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
            _normalGameplaySet = new PlayerGameplayActionSet(
                _playerCharacter,
                hackConfig.JumpAction,
                hackConfig.NormalAttack,
                hackConfig.SecondaryAttack,
                hackConfig.InteractAction,
                hackConfig.SkillMode1,
                hackConfig.SkillMode2);
            _currentActionSet = _normalGameplaySet;
        }

        private void SubscribeToInputController() {
            InputController.Instance.OnBtn1Pressed += OnBtn1Pressed;
            InputController.Instance.OnBtn1Held += OnBtn1Held;
            InputController.Instance.OnBtn1Released += OnBtn1Released;

            InputController.Instance.OnBtn2Pressed += OnBtn2Pressed;
            InputController.Instance.OnBtn2Held += OnBtn2Held;
            InputController.Instance.OnBtn2Released += OnBtn2Released;

            InputController.Instance.OnBtn3Pressed += OnBtn3Pressed;
            InputController.Instance.OnBtn3Held += OnBtn3Held;
            InputController.Instance.OnBtn3Released += OnBtn3Released;

            InputController.Instance.OnBtn4Pressed += OnBtn4Pressed;
            InputController.Instance.OnBtn4Held += OnBtn4Held;
            InputController.Instance.OnBtn4Released += OnBtn4Released;

            InputController.Instance.OnLTriggerPressed += OnLTriggerPressed;
            InputController.Instance.OnLTriggerHeld += OnLTriggerHeld;
            InputController.Instance.OnLTriggerReleased += OnLTriggerReleased;

            InputController.Instance.OnRTriggerPressed += OnRTriggerPressed;
            InputController.Instance.OnRTriggerHeld += OnRTriggerHeld;
            InputController.Instance.OnRTriggerReleased += OnRTriggerReleased;
        }

        private void SubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated += OnCharacterAnimationStatusSent;
        }

        private void SubscribeToDamageable() {
            _playerCharacter.Damageable.OnHit += OnDamageableHit;
        }

        private void UnsubscribeToInputController() {
            InputController.Instance.OnBtn1Pressed -= OnBtn1Pressed;
            InputController.Instance.OnBtn1Held -= OnBtn1Held;
            InputController.Instance.OnBtn1Released -= OnBtn1Released;

            InputController.Instance.OnBtn2Pressed -= OnBtn2Pressed;
            InputController.Instance.OnBtn2Held -= OnBtn2Held;
            InputController.Instance.OnBtn2Released -= OnBtn2Released;

            InputController.Instance.OnBtn3Pressed -= OnBtn3Pressed;
            InputController.Instance.OnBtn3Held -= OnBtn3Held;
            InputController.Instance.OnBtn3Released -= OnBtn3Released;

            InputController.Instance.OnBtn4Pressed -= OnBtn4Pressed;
            InputController.Instance.OnBtn4Held -= OnBtn4Held;
            InputController.Instance.OnBtn4Released -= OnBtn4Released;

            InputController.Instance.OnLTriggerPressed -= OnLTriggerPressed;
            InputController.Instance.OnLTriggerHeld -= OnLTriggerHeld;
            InputController.Instance.OnLTriggerReleased += OnLTriggerReleased;

            InputController.Instance.OnRTriggerPressed -= OnRTriggerPressed;
            InputController.Instance.OnRTriggerHeld -= OnRTriggerHeld;
            InputController.Instance.OnRTriggerReleased -= OnRTriggerReleased;
        }

        private void UnsubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated -= OnCharacterAnimationStatusSent;
        }

        private void UnsubscribeToDamageable() {
            _playerCharacter.Damageable.OnHit -= OnDamageableHit;
        }

        private void OnBtn1Pressed() {
            if(_currentActionSet.Btn1Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill_Normal.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Held() {
            if (_currentActionSet.Btn1Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill_Normal.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn1Released() {
            if (_currentActionSet.Btn1Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn1Skill_Normal.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Pressed() {
            if(_currentActionSet.Btn2Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill_Normal.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Held() {
            if (_currentActionSet.Btn2Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill_Normal.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn2Released() {
            if (_currentActionSet.Btn2Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn2Skill_Normal.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn3Pressed() {
            if(_currentActionSet.Btn3Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn3Skill_Normal.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn3Held() {
            if (_currentActionSet.Btn3Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn3Skill_Normal.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn3Released() {
            if (_currentActionSet.Btn3Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn3Skill_Normal.Release(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn4Pressed() {
            if (_currentActionSet.Btn4Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn4Skill_Normal.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn4Held() {
            if (_currentActionSet.Btn4Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn4Skill_Normal.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnBtn4Released() {
            if (_currentActionSet.Btn4Skill_Normal == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.Btn4Skill_Normal.Release(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnLTriggerPressed() {
            if (_currentActionSet.SkillMode1 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode1.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnLTriggerHeld() {
            if (_currentActionSet.SkillMode1 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode1.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnLTriggerReleased() {
            if (_currentActionSet.SkillMode1 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode1.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnRTriggerPressed() {
            if (_currentActionSet.SkillMode2 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode2.Initiate(_playerCharacter, CurrentState, CharacterActionContext.Initiate);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnRTriggerHeld() {
            if (_currentActionSet.SkillMode2 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode2.Hold(_playerCharacter, CurrentState, CharacterActionContext.Hold);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void OnRTriggerReleased() {
            if (_currentActionSet.SkillMode2 == null) {
                return;
            }
            CharacterActionResponse response = _currentActionSet.SkillMode2.Release(_playerCharacter, CurrentState, CharacterActionContext.Release);
            if (response.Success) {
                PerformActionSuccess(response.State);
            }
        }

        private void PerformActionSuccess(ICharacterActionState state) {
            CurrentState?.Clear();
            if(state != null) {
                CurrentState = state;
                _playerCharacter.AnimationController.UpdateAnimationState(CurrentState?.AnimationData);
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

        private void OnDamageableHit(HitEventInfo info) {
            CurrentState?.Clear();
        }
    }
}
