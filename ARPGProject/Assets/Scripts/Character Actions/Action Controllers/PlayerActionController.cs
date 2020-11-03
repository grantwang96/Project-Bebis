using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Bebis {
    public class PlayerActionController : MonoBehaviour, IActionController {

        /*
        [SerializeField] private PlayerCharacter _playerCharacter;

        public ActionPermissions Permissions => CurrentState?.Permissions ?? ActionPermissions.All;
        public ICharacterActionState CurrentState { get; private set; }
        public IPlayerGameplayActionSet CurrentActionSet { get; private set; }

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action OnPerformActionSuccess;
   
        public event Action OnCurrentActionSetUpdated;
        public event Action OnUpdate;

        private CharacterActionData _bufferedActionData;
        private CharacterActionContext _bufferedActionContext;
        private bool _hasBufferedAction = false;
        private List<CharacterActionData> _currentHoldingActions = new List<CharacterActionData>();

        private PlayerActionInfoProvider _playerActionInfoProvider;

        public int ActionRestrictions { get; private set; }

        private void Start() {
            SetGameplayActionSets();
            SubscribeToDamageable();
            SubscribeToAnimationController();
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutSet += OnSkillsLoadoutUpdated;

            ActionRestrictions = 0;
        }

        public void Initialized() {
            SetGameplayActionSets();
            SubscribeToDamageable();
            SubscribeToAnimationController();
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutSet += OnSkillsLoadoutUpdated;

            ActionRestrictions = 0;
        }

        // set the player's action sets
        private void SetGameplayActionSets() {
            OnCurrentActionSetUpdated?.Invoke();
        }

        private void OnSkillsLoadoutUpdated(IPlayerSkillsLoadout skillsLoadout) {

        }

        #region SUBCRIBING/UNSUBSCRIBING TO EVENTS

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

        private bool TryPerformAction(CharacterActionData data, CharacterActionContext context) {
            // do not perform any actions if restricted
            if (ActionRestrictions > 0) {
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

        // force clear the action from an external source (ex. getting hit)
        public void ClearCurrentActionState() {
            CurrentState?.Clear();
            OnActionStatusUpdated?.Invoke(ActionStatus.Completed);
            _bufferedActionData = null;
            _hasBufferedAction = false;
            CurrentState = null;
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
            if (CurrentState != null) {
                // update the current action status
                CurrentState.Status = status;
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
            OnActionStatusUpdated?.Invoke(status);
        }

        // upon taking entering hitstun
        private void OnDamageableHitStun(HitEventInfo info) {
            ClearCurrentActionState();
            _playerCharacter.AnimationController.OnAnimationStateUpdated += OnDamageableAnimationCompleted;
            ActionRestrictions++;
        }

        // upon finishing hitstun animation
        private void OnDamageableAnimationCompleted(AnimationState state) {
            if(state != AnimationState.Completed) {
                return;
            }
            _playerCharacter.AnimationController.OnAnimationStateUpdated -= OnDamageableAnimationCompleted;
            ActionRestrictions = Mathf.Max(0, ActionRestrictions - 1);
        }
        */

        public ICharacterActionState CurrentState { get; private set; }

        public event Action OnCurrentActionUpdated;

        [SerializeField] private PlayerCharacter _playerCharacter;

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
            if(data == null) {
                Debug.LogWarning($"[{name}]: Attempted action data was null!");
                return;
            }
            // check if we have a state registered for this data
            bool containsState = _currentActionStates.TryGetValue(data, out ICharacterActionState characterActionState);
            CharacterActionResponse response = new CharacterActionResponse();
            // process the interaction with the action data
            switch (context) {
                case CharacterActionContext.Initiate:
                    response = data.Initiate(_playerCharacter, characterActionState, context);
                    break;
                case CharacterActionContext.Hold:
                    response = data.Hold(_playerCharacter, characterActionState, context);
                    break;
                case CharacterActionContext.Release:
                    response = data.Release(_playerCharacter, characterActionState, context);
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
            characterActionState.Clear();
            characterActionState.OnActionStatusUpdated -= OnActionStatusUpdated;
            _currentActionStates.Remove(characterActionState.Data);
            if (CurrentState == characterActionState) {
                CurrentState = null;
                OnCurrentActionUpdated?.Invoke();
            }
        }
    }
}
