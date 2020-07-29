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

        private CharacterActionData _bufferedActionData;
        private CharacterActionContext _bufferedActionContext;
        private bool _hasBufferedAction = false;

        private int _actionRestrictions;

        private void Start() {
            SetGameplayActionSets();
            SubscribeToInputController();
            SubscribeToDamageable();
            SubscribeToAnimationController();

            _actionRestrictions = 0;
        }

        // set the player's action sets
        private void SetGameplayActionSets() {
            // TODO: populate action sets with data propogated from manager/persistence
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

        #region SUBCRIBING/UNSUBSCRIBING TO EVENTS
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
            _playerCharacter.Damageable.OnHitStun += OnDamageableHitStun;
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
            _playerCharacter.Damageable.OnHit -= OnDamageableHitStun;
        }
        #endregion

        #region APPLYING INPUTS TO CHARACTER ACTIONS
        private void OnBtn1Pressed() {
            TryPerformAction(_currentActionSet.Btn1Skill, CharacterActionContext.Initiate);
        }

        private void OnBtn1Held() {
            TryPerformAction(_currentActionSet.Btn1Skill, CharacterActionContext.Hold);
        }

        private void OnBtn1Released() {
            TryPerformAction(_currentActionSet.Btn1Skill, CharacterActionContext.Release);
        }

        private void OnBtn2Pressed() {
            bool se = TryPerformAction(_currentActionSet.Btn2Skill, CharacterActionContext.Initiate);
        }

        private void OnBtn2Held() {
            TryPerformAction(_currentActionSet.Btn2Skill, CharacterActionContext.Hold);
        }

        private void OnBtn2Released() {
            TryPerformAction(_currentActionSet.Btn2Skill, CharacterActionContext.Release);
        }

        private void OnBtn3Pressed() {
            TryPerformAction(_currentActionSet.Btn3Skill, CharacterActionContext.Initiate);
        }

        private void OnBtn3Held() {
            TryPerformAction(_currentActionSet.Btn3Skill, CharacterActionContext.Hold);
        }

        private void OnBtn3Released() {
            TryPerformAction(_currentActionSet.Btn3Skill, CharacterActionContext.Release);
        }

        private void OnBtn4Pressed() {
            TryPerformAction(_currentActionSet.Btn4Skill, CharacterActionContext.Initiate);
        }

        private void OnBtn4Held() {
            TryPerformAction(_currentActionSet.Btn4Skill, CharacterActionContext.Hold);
        }

        private void OnBtn4Released() {
            TryPerformAction(_currentActionSet.Btn4Skill, CharacterActionContext.Release);
        }

        private void OnLTriggerPressed() {
            TryPerformAction(_currentActionSet.SkillMode1, CharacterActionContext.Initiate);
        }

        private void OnLTriggerHeld() {
            TryPerformAction(_currentActionSet.SkillMode1, CharacterActionContext.Hold);
        }

        private void OnLTriggerReleased() {
            TryPerformAction(_currentActionSet.SkillMode1, CharacterActionContext.Release);
        }

        private void OnRTriggerPressed() {
            // TODO: set to skill mode 2
            TryPerformAction(_currentActionSet.SkillMode2, CharacterActionContext.Initiate);
        }

        private void OnRTriggerHeld() {
            // TODO: set to skill mode 2
            TryPerformAction(_currentActionSet.SkillMode2, CharacterActionContext.Hold);
        }

        private void OnRTriggerReleased() {
            // TODO: set to skill mode 2
            TryPerformAction(_currentActionSet.SkillMode2, CharacterActionContext.Release);
        }
        #endregion

        private bool TryPerformAction(CharacterActionData data, CharacterActionContext context) {
            // do not perform any actions if restricted
            if (_actionRestrictions > 0) {
                return false;
            }
            // if the given action is null do not do anything either
            if(data == null) {
                return false;
            }
            // send request to data object to perform the action
            CharacterActionResponse response = null;
            switch (context) {
                case CharacterActionContext.Initiate:
                    response = data.Initiate(_playerCharacter, CurrentState, context);
                    break;
                case CharacterActionContext.Hold:
                    response = data.Hold(_playerCharacter, CurrentState, context);
                    break;
                case CharacterActionContext.Release:
                    response = data.Release(_playerCharacter, CurrentState, context);
                    break;
                default:
                    CustomLogger.Error(nameof(PlayerActionController), $"Unhandled action context: {context}");
                    break;
            }
            bool success = response != null && response.Success;
            // upon success, execute action in action state
            if (success) {
                PerformActionSuccess(response.State);
                return true;
            }
            // attempt to buffer the action if unsuccessful
            if (CanBufferAction(response)) {
                TryBufferAction(data, context);
            }
            return false;
        }

        private bool CanBufferAction(CharacterActionResponse response) {
            return CurrentState != null &&
                CurrentState.Status.HasFlag(ActionStatus.CanBufferInput)
                && !_hasBufferedAction
                && response.Bufferable;
        }

        // sets an action to be buffered
        private void TryBufferAction(CharacterActionData actionData, CharacterActionContext context) {
            _bufferedActionData = actionData;
            _bufferedActionContext = context;
            _hasBufferedAction = true;
        }

        // performs the buffered action
        private void PerformBufferedAction() {
            if (TryPerformAction(_bufferedActionData, _bufferedActionContext)) {
                _hasBufferedAction = false;
            }
        }

        // called from external source to force an action to be performed
        public bool PerformAction(CharacterActionData actionData, CharacterActionContext context) {
            return TryPerformAction(actionData, context);
        }

        // upon successful action, update action and animation state
        private void PerformActionSuccess(ICharacterActionState state) {
            CurrentState?.Clear();
            if(state != null) {
                CurrentState = state;
                _playerCharacter.AnimationController.UpdateAnimationState(CurrentState?.AnimationData);
                OnPerformActionSuccess?.Invoke();
                OnCharacterAnimationStatusSent(CurrentState.Status);
            }
        }
        
        // listens for action status updates from character animations
        private void OnCharacterAnimationStatusSent(ActionStatus status) {
            if (CurrentState == null) {
                return;
            }
            // update the current action status
            CurrentState.Status = status;
            OnActionStatusUpdated?.Invoke(status);
            // if state can be transitioned or is completed
            if (CurrentState.Status.HasFlag(ActionStatus.CanTransition) || CurrentState.Status.HasFlag(ActionStatus.Completed)) {
                // if action is completed, reset state
                if (CurrentState.Status.HasFlag(ActionStatus.Completed)) {
                    CurrentState.Clear();
                    CurrentState = null;
                }
                // if a buffered action is set, trigger
                if (_hasBufferedAction) {
                    PerformBufferedAction();
                }
            }
        }

        // upon taking entering hitstun
        private void OnDamageableHitStun(HitEventInfo info) {
            CurrentState?.Clear();
            _playerCharacter.AnimationController.OnAnimationStateUpdated += OnDamageableAnimationCompleted;
            _actionRestrictions++;
        }

        // upon finishing hitstun animation
        private void OnDamageableAnimationCompleted(AnimationState state) {
            if(state != AnimationState.Completed) {
                return;
            }
            _playerCharacter.AnimationController.OnAnimationStateUpdated -= OnDamageableAnimationCompleted;
            _actionRestrictions = Mathf.Max(0, _actionRestrictions - 1);
        }
    }
}
