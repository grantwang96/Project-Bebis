using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Bebis {
    public class PlayerActionController : MonoBehaviour, IActionController {

        private const string ActionButton1Id = "ActionButton1";
        private const string ActionButton2Id = "ActionButton2";
        private const string ActionButton3Id = "ActionButton3";
        private const string ActionButton4Id = "ActionButton4";
        private const string SkillsButton1Id = "SkillsButton1";
        private const string SkillsButton2Id = "SkillsButton2";

        [SerializeField] private PlayerCharacter _playerCharacter;

        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;

        private IPlayerGameplayActionSet _normalGameplaySet;
        private IPlayerGameplayActionSet _skillMode1GameplaySet;
        private IPlayerGameplayActionSet _skillMode2GameplaySet;
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
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutSet += OnSkillsLoadoutUpdated;

            _actionRestrictions = 0;
        }

        // set the player's action sets
        private void SetGameplayActionSets() {
            _normalGameplaySet = new PlayerNormalGameplayActionSet(_playerCharacter, PlayerCharacterManager.Instance.SkillsLoadout);
            _skillMode1GameplaySet = new PlayerSkillsActionSet(true, PlayerCharacterManager.Instance.SkillsLoadout);
            _skillMode2GameplaySet = new PlayerSkillsActionSet(false, PlayerCharacterManager.Instance.SkillsLoadout);
            _currentActionSet = _normalGameplaySet;
        }

        private void OnSkillsLoadoutUpdated(IPlayerSkillsLoadout skillsLoadout) {
            _normalGameplaySet.Override(skillsLoadout);
            _skillMode1GameplaySet.Override(skillsLoadout);
            _skillMode2GameplaySet.Override(skillsLoadout);
        }

        #region SUBCRIBING/UNSUBSCRIBING TO EVENTS

        private void SubscribeToInputController() {
            InputController.Instance.PlayerInputActionMap[ActionButton1Id].started += HandleActionButton1;
            InputController.Instance.PlayerInputActionMap[ActionButton1Id].performed += HandleActionButton1;
            InputController.Instance.PlayerInputActionMap[ActionButton1Id].canceled += HandleActionButton1;

            InputController.Instance.PlayerInputActionMap[ActionButton2Id].started += HandleActionButton2;
            InputController.Instance.PlayerInputActionMap[ActionButton2Id].performed += HandleActionButton2;
            InputController.Instance.PlayerInputActionMap[ActionButton2Id].canceled += HandleActionButton2;

            InputController.Instance.PlayerInputActionMap[ActionButton3Id].started += HandleActionButton3;
            InputController.Instance.PlayerInputActionMap[ActionButton3Id].performed += HandleActionButton3;
            InputController.Instance.PlayerInputActionMap[ActionButton3Id].canceled += HandleActionButton3;

            InputController.Instance.PlayerInputActionMap[ActionButton4Id].started += HandleActionButton4;
            InputController.Instance.PlayerInputActionMap[ActionButton4Id].performed += HandleActionButton4;
            InputController.Instance.PlayerInputActionMap[ActionButton4Id].canceled += HandleActionButton4;

            InputController.Instance.PlayerInputActionMap[SkillsButton1Id].started += HandleSkillsButton1;
            InputController.Instance.PlayerInputActionMap[SkillsButton1Id].performed += HandleSkillsButton1;
            InputController.Instance.PlayerInputActionMap[SkillsButton1Id].canceled += HandleSkillsButton1;

            InputController.Instance.PlayerInputActionMap[SkillsButton2Id].started += HandleSkillsButton2;
            InputController.Instance.PlayerInputActionMap[SkillsButton2Id].performed += HandleSkillsButton2;
            InputController.Instance.PlayerInputActionMap[SkillsButton2Id].canceled += HandleSkillsButton2;
        }

        private void SubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated += OnCharacterAnimationStatusSent;
        }

        private void SubscribeToDamageable() {
            _playerCharacter.Damageable.OnHitStun += OnDamageableHitStun;
        }
        
        private void UnsubscribeToAnimationController() {
            _playerCharacter.AnimationController.OnActionStatusUpdated -= OnCharacterAnimationStatusSent;
        }

        private void UnsubscribeToDamageable() {
            _playerCharacter.Damageable.OnHit -= OnDamageableHitStun;
        }
        #endregion

        #region APPLYING INPUTS TO CHARACTER ACTIONS

        private void HandleActionButton1(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.Btn1Skill;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            TryPerformAction(actionData, context);
        }

        private void HandleActionButton2(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.Btn2Skill;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            TryPerformAction(actionData, context);
        }

        private void HandleActionButton3(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.Btn3Skill;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            TryPerformAction(actionData, context);
        }

        private void HandleActionButton4(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.Btn4Skill;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            TryPerformAction(actionData, context);
        }

        private void HandleSkillsButton1(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.SkillMode1;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            switch (context) {
                case CharacterActionContext.Initiate:
                    _currentActionSet = _skillMode1GameplaySet;
                    break;
                case CharacterActionContext.Release:
                    _currentActionSet = _normalGameplaySet;
                    break;
                default: break;
            }
            TryPerformAction(actionData, context);
        }

        private void HandleSkillsButton2(InputAction.CallbackContext callbackContext) {
            CharacterActionData actionData = _currentActionSet.SkillMode2;
            CharacterActionContext context = GetCharacterActionContext(callbackContext);
            TryPerformAction(actionData, context);
        }

        private static CharacterActionContext GetCharacterActionContext(InputAction.CallbackContext callbackContext) {
            if (callbackContext.started) {
                return CharacterActionContext.Initiate;
            } else if (callbackContext.performed) {
                return CharacterActionContext.Hold;
            } else if (callbackContext.canceled) {
                return CharacterActionContext.Release;
            }
            return CharacterActionContext.Invalid;
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
