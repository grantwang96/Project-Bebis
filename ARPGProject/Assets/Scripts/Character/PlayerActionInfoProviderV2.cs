using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Bebis
{
    public class PlayerActionInfoProviderV2
    {
        // input ids for actions
        private const string ActionSouthPressedId = "ActionSouthPressed";
        private const string ActionSouthHeldId = "ActionSouthHeld";
        private const string ActionSouthReleasedId = "ActionSouthReleased";

        private const string ActionWestPressedId = "ActionWestPressed";
        private const string ActionWestHeldId = "ActionWestHeld";
        private const string ActionWestReleasedId = "ActionWestReleased";

        private const string ActionNorthPressedId = "ActionNorthPressed";
        private const string ActionNorthHeldId = "ActionNorthHeld";
        private const string ActionNorthReleased = "ActionNorthReleased";

        private const string ActionEastPressedId = "ActionEastPressed";
        private const string ActionEastHeldId = "ActionEastHeld";
        private const string ActionEastReleasedId = "ActionEastReleased";

        private const string SkillSet1PressedId = "SkillsButton1Pressed";
        private const string SkillSet1HeldId = "SkillsButton1Held";
        private const string SkillSet1ReleasedId = "SkillsButton1Released";

        private const string SkillSet2PressedId = "SkillsButton2Pressed";
        private const string SkillSet2HeldId = "SkillsButton2Held";
        private const string SkillSet2ReleasedId = "SkillsButton2Released";
        public IPlayerActionSet CurrentActionSet { get; private set; }

        // action controller implementations
        public event Action<string, CharacterActionContext> OnActionAttempted;
        public event Action OnCurrentActionSetUpdated;

        // private IPlayerActionSetProvider _playerActionSetProvider;
        private IPlayerActionSet _normalActionSet;
        private IPlayerActionSet _skills1ActionSet;
        private IPlayerActionSet _skills2ActionSet;
        private string _rightTriggerActionId;
        private string _leftTriggerActionId;
        private ICharacterV2 _playerCharacter;

        public PlayerActionInfoProviderV2(ICharacterV2 playerCharacter) {
            _playerCharacter = playerCharacter;
            // initialize move sets here
            _normalActionSet = new PlayerNormalActionSet(PlayerCharacterManager.Instance.SkillsLoadoutV2, _playerCharacter);
            _skills1ActionSet = new PlayerSkillsActionSetV2(PlayerCharacterManager.Instance.SkillsLoadoutV2, _playerCharacter, true);
            _skills2ActionSet = new PlayerSkillsActionSetV2(PlayerCharacterManager.Instance.SkillsLoadoutV2, _playerCharacter, false);
            UpdateLoadout(PlayerCharacterManager.Instance.SkillsLoadoutV2);
            CurrentActionSet = _normalActionSet;
            SubscribeToInputEvents();
            SubscribeToActionsUpdate();
        }

        public void UpdateLoadout(IPlayerSkillsLoadoutV2 playerSkillsLoadout) {
            _normalActionSet.Update(playerSkillsLoadout, _playerCharacter);
            _skills1ActionSet.Update(playerSkillsLoadout, _playerCharacter);
            _skills2ActionSet.Update(playerSkillsLoadout, _playerCharacter);
            _rightTriggerActionId = playerSkillsLoadout.RightTriggerSkill?.Id;
            _leftTriggerActionId = playerSkillsLoadout.LeftTriggerSkill?.Id;
        }

        public void SubscribeToInputEvents() {
            InputController.Instance.PlayerInputActionMap[ActionSouthPressedId].performed += OnActionSouthPressed;
            InputController.Instance.PlayerInputActionMap[ActionSouthHeldId].performed += OnActionSouthHeld;
            InputController.Instance.PlayerInputActionMap[ActionSouthReleasedId].performed += OnActionSouthReleased;

            InputController.Instance.PlayerInputActionMap[ActionWestPressedId].performed += OnActionWestPressed;
            InputController.Instance.PlayerInputActionMap[ActionWestHeldId].performed += OnActionWestHeld;
            InputController.Instance.PlayerInputActionMap[ActionWestReleasedId].performed += OnActionWestReleased;

            InputController.Instance.PlayerInputActionMap[ActionNorthPressedId].performed += OnActionNorthPressed;
            InputController.Instance.PlayerInputActionMap[ActionNorthHeldId].performed += OnActionNorthHeld;
            InputController.Instance.PlayerInputActionMap[ActionNorthReleased].performed += OnActionNorthReleased;

            InputController.Instance.PlayerInputActionMap[ActionEastPressedId].performed += OnActionEastPressed;
            InputController.Instance.PlayerInputActionMap[ActionEastHeldId].performed += OnActionEastHeld;
            InputController.Instance.PlayerInputActionMap[ActionEastReleasedId].performed += OnActionEastReleased;

            InputController.Instance.PlayerInputActionMap[SkillSet1PressedId].performed += OnRightTriggerPressed;
            InputController.Instance.PlayerInputActionMap[SkillSet1HeldId].performed += OnRightTriggerHeld;
            InputController.Instance.PlayerInputActionMap[SkillSet1ReleasedId].performed += OnRightTriggerReleased;

            InputController.Instance.PlayerInputActionMap[SkillSet2PressedId].performed += OnLeftTriggerPressed;
            InputController.Instance.PlayerInputActionMap[SkillSet2HeldId].performed += OnLeftTriggerHeld;
            InputController.Instance.PlayerInputActionMap[SkillSet2ReleasedId].performed += OnLeftTriggerReleased;
        }

        private void SubscribeToActionsUpdate() {
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutV2Set += UpdateLoadout;
        }

        public void UnsubscribeToInputEvents() {
            InputController.Instance.PlayerInputActionMap[ActionSouthPressedId].performed -= OnActionSouthPressed;
            InputController.Instance.PlayerInputActionMap[ActionSouthHeldId].performed -= OnActionSouthHeld;
            InputController.Instance.PlayerInputActionMap[ActionSouthReleasedId].performed -= OnActionSouthReleased;

            InputController.Instance.PlayerInputActionMap[ActionWestPressedId].performed -= OnActionWestPressed;
            InputController.Instance.PlayerInputActionMap[ActionWestHeldId].performed -= OnActionWestHeld;
            InputController.Instance.PlayerInputActionMap[ActionWestReleasedId].performed -= OnActionWestReleased;

            InputController.Instance.PlayerInputActionMap[ActionNorthPressedId].performed -= OnActionNorthPressed;
            InputController.Instance.PlayerInputActionMap[ActionNorthHeldId].performed -= OnActionNorthHeld;
            InputController.Instance.PlayerInputActionMap[ActionNorthReleased].performed -= OnActionNorthReleased;

            InputController.Instance.PlayerInputActionMap[ActionEastPressedId].performed -= OnActionEastPressed;
            InputController.Instance.PlayerInputActionMap[ActionEastHeldId].performed -= OnActionEastHeld;
            InputController.Instance.PlayerInputActionMap[ActionEastReleasedId].performed -= OnActionEastReleased;

            InputController.Instance.PlayerInputActionMap[SkillSet1PressedId].performed -= OnRightTriggerPressed;
            InputController.Instance.PlayerInputActionMap[SkillSet1HeldId].performed -= OnRightTriggerHeld;
            InputController.Instance.PlayerInputActionMap[SkillSet1ReleasedId].performed -= OnRightTriggerReleased;

            InputController.Instance.PlayerInputActionMap[SkillSet2PressedId].performed -= OnLeftTriggerPressed;
            InputController.Instance.PlayerInputActionMap[SkillSet2HeldId].performed -= OnLeftTriggerHeld;
            InputController.Instance.PlayerInputActionMap[SkillSet2ReleasedId].performed -= OnLeftTriggerReleased;
        }

        private void UnsubscribeToActionsUpdate() {
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutV2Set -= UpdateLoadout;
        }

        private void AttemptAction(string actionId, CharacterActionContext interaction) {
            if (string.IsNullOrEmpty(actionId)) {
                return;
            }
            OnActionAttempted?.Invoke(actionId, interaction);
        }

        private void OnActionSouthPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonSouthAction, CharacterActionContext.Initiate);
        }

        private void OnActionSouthHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += ActionSouthHold;
        }

        private void ActionSouthHold() {
            AttemptAction(CurrentActionSet.ButtonSouthAction, CharacterActionContext.Hold);
        }

        private void OnActionSouthReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonSouthAction, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= ActionSouthHold;
        }

        private void OnActionWestPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonWestAction, CharacterActionContext.Initiate);
        }

        private void OnActionWestHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += ActionWestHold;
        }

        private void ActionWestHold() {
            AttemptAction(CurrentActionSet.ButtonWestAction, CharacterActionContext.Hold);
        }

        private void OnActionWestReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonWestAction, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= ActionWestHold;
        }

        private void OnActionNorthPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonNorthAction, CharacterActionContext.Initiate);
        }

        private void OnActionNorthHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += ActionNorthHold;
        }

        private void ActionNorthHold() {
            AttemptAction(CurrentActionSet.ButtonNorthAction, CharacterActionContext.Hold);
        }

        private void OnActionNorthReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonNorthAction, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= ActionNorthHold;
        }

        private void OnActionEastPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonEastAction, CharacterActionContext.Initiate);
        }

        private void OnActionEastHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += ActionEastHold;
        }

        private void ActionEastHold() {
            AttemptAction(CurrentActionSet.ButtonEastAction, CharacterActionContext.Hold);
        }

        private void OnActionEastReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.ButtonEastAction, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= ActionEastHold;
        }

        private void OnRightTriggerPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(_rightTriggerActionId, CharacterActionContext.Initiate);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills1ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnRightTriggerHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += SkillSetRightTriggerHeld;
        }

        private void SkillSetRightTriggerHeld() {
            AttemptAction(_rightTriggerActionId, CharacterActionContext.Hold);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills1ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnRightTriggerReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(_rightTriggerActionId, CharacterActionContext.Release);
            if (CurrentActionSet == _skills1ActionSet) {
                CurrentActionSet = _normalActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
            MonoBehaviourMaster.Instance.OnFixedUpdate -= SkillSetRightTriggerHeld;
        }

        private void OnLeftTriggerPressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(_leftTriggerActionId, CharacterActionContext.Initiate);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills2ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnLeftTriggerHeld(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += SkillSetLeftTriggerHeld;
        }

        private void SkillSetLeftTriggerHeld() {
            AttemptAction(_leftTriggerActionId, CharacterActionContext.Hold);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills2ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnLeftTriggerReleased(InputAction.CallbackContext callbackContext) {
            AttemptAction(_leftTriggerActionId, CharacterActionContext.Release);
            if (CurrentActionSet == _skills2ActionSet) {
                CurrentActionSet = _normalActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
            MonoBehaviourMaster.Instance.OnFixedUpdate -= SkillSetLeftTriggerHeld;
        }
    }
}
