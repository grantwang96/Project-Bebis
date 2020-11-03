using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Bebis {
    public class PlayerActionInfoProvider {
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
        
        // action controller implementations
        public event Action<ICharacterActionData, CharacterActionContext> OnActionAttempted;
        public event Action OnCurrentActionSetUpdated;

        // private IPlayerActionSetProvider _playerActionSetProvider;
        private PlayerCharacter _playerCharacter;
        private IPlayerGameplayActionSet _normalActionSet;
        private IPlayerGameplayActionSet _skills1ActionSet;
        private IPlayerGameplayActionSet _skills2ActionSet;
        public IPlayerGameplayActionSet CurrentActionSet { get; private set; }

        public PlayerActionInfoProvider(PlayerCharacter character) {
            _playerCharacter = character;
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutSet += UpdateLoadout;
            UpdateLoadout(PlayerCharacterManager.Instance.SkillsLoadout);
            CurrentActionSet = _normalActionSet;
            SubscribeToInputEvents();
        }

        private void UpdateLoadout(IPlayerSkillsLoadout loadout) {
            _normalActionSet = new PlayerNormalGameplayActionSet(_playerCharacter, loadout);
            _skills1ActionSet = new PlayerSkillsActionSet(true, loadout);
            _skills2ActionSet = new PlayerSkillsActionSet(false, loadout);
        }

        public void SubscribeToInputEvents() {
            InputController.Instance.PlayerInputActionMap[ActionSouthPressedId].performed += OnAction1Pressed;
            InputController.Instance.PlayerInputActionMap[ActionSouthHeldId].performed += OnAction1Held;
            InputController.Instance.PlayerInputActionMap[ActionSouthReleasedId].performed += OnAction1Released;

            InputController.Instance.PlayerInputActionMap[ActionWestPressedId].performed += OnAction2Pressed;
            InputController.Instance.PlayerInputActionMap[ActionWestHeldId].performed += OnAction2Held;
            InputController.Instance.PlayerInputActionMap[ActionWestReleasedId].performed += OnAction2Released;

            InputController.Instance.PlayerInputActionMap[ActionNorthPressedId].performed += OnAction3Pressed;
            InputController.Instance.PlayerInputActionMap[ActionNorthHeldId].performed += OnAction3Held;
            InputController.Instance.PlayerInputActionMap[ActionNorthReleased].performed += OnAction3Released;

            InputController.Instance.PlayerInputActionMap[ActionEastPressedId].performed += OnAction4Pressed;
            InputController.Instance.PlayerInputActionMap[ActionEastHeldId].performed += OnAction4Held;
            InputController.Instance.PlayerInputActionMap[ActionEastReleasedId].performed += OnAction4Released;

            InputController.Instance.PlayerInputActionMap[SkillSet1PressedId].performed += OnSkillSet1Pressed;
            InputController.Instance.PlayerInputActionMap[SkillSet1HeldId].performed += OnSkillSet1Held;
            InputController.Instance.PlayerInputActionMap[SkillSet1ReleasedId].performed += OnSkillSet1Released;

            InputController.Instance.PlayerInputActionMap[SkillSet2PressedId].performed += OnSkillSet2Pressed;
            InputController.Instance.PlayerInputActionMap[SkillSet2HeldId].performed += OnSkillSet2Held;
            InputController.Instance.PlayerInputActionMap[SkillSet2ReleasedId].performed += OnSkillSet2Released;
        }

        public void UnsubscribeToInputEvents() {
            InputController.Instance.PlayerInputActionMap[ActionSouthPressedId].performed -= OnAction1Pressed;
            InputController.Instance.PlayerInputActionMap[ActionSouthHeldId].performed -= OnAction1Held;
            InputController.Instance.PlayerInputActionMap[ActionSouthReleasedId].performed -= OnAction1Released;

            InputController.Instance.PlayerInputActionMap[ActionWestPressedId].performed -= OnAction2Pressed;
            InputController.Instance.PlayerInputActionMap[ActionWestHeldId].performed -= OnAction2Held;
            InputController.Instance.PlayerInputActionMap[ActionWestReleasedId].performed -= OnAction2Released;

            InputController.Instance.PlayerInputActionMap[ActionNorthPressedId].performed -= OnAction3Pressed;
            InputController.Instance.PlayerInputActionMap[ActionNorthHeldId].performed -= OnAction3Held;
            InputController.Instance.PlayerInputActionMap[ActionNorthReleased].performed -= OnAction3Released;

            InputController.Instance.PlayerInputActionMap[ActionEastPressedId].performed -= OnAction4Pressed;
            InputController.Instance.PlayerInputActionMap[ActionEastHeldId].performed -= OnAction4Held;
            InputController.Instance.PlayerInputActionMap[ActionEastReleasedId].performed -= OnAction4Released;

            InputController.Instance.PlayerInputActionMap[SkillSet1PressedId].performed -= OnSkillSet1Pressed;
            InputController.Instance.PlayerInputActionMap[SkillSet1HeldId].performed -= OnSkillSet1Held;
            InputController.Instance.PlayerInputActionMap[SkillSet1ReleasedId].performed -= OnSkillSet1Released;

            InputController.Instance.PlayerInputActionMap[SkillSet2PressedId].performed -= OnSkillSet2Pressed;
            InputController.Instance.PlayerInputActionMap[SkillSet2HeldId].performed -= OnSkillSet2Held;
            InputController.Instance.PlayerInputActionMap[SkillSet2ReleasedId].performed -= OnSkillSet2Released;
        }

        private void OnAction1Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn1Skill, CharacterActionContext.Initiate);
        }

        private void OnAction1Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += Action1Hold;
        }

        private void Action1Hold() {
            AttemptAction(CurrentActionSet.Btn1Skill, CharacterActionContext.Hold);
        }

        private void OnAction1Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn1Skill, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= Action1Hold;
        }

        private void OnAction2Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn2Skill, CharacterActionContext.Initiate);
        }

        private void OnAction2Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += Action2Hold;
        }

        private void Action2Hold() {
            AttemptAction(CurrentActionSet.Btn2Skill, CharacterActionContext.Hold);
        }

        private void OnAction2Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn2Skill, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= Action2Hold;
        }

        private void OnAction3Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn3Skill, CharacterActionContext.Initiate);
        }

        private void OnAction3Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += Action3Hold;
        }

        private void Action3Hold() {
            AttemptAction(CurrentActionSet.Btn3Skill, CharacterActionContext.Hold);
        }

        private void OnAction3Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn3Skill, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= Action3Hold;
        }

        private void OnAction4Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn4Skill, CharacterActionContext.Initiate);
        }

        private void OnAction4Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += Action4Hold;
        }

        private void Action4Hold() {
            AttemptAction(CurrentActionSet.Btn3Skill, CharacterActionContext.Hold);
        }

        private void OnAction4Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.Btn4Skill, CharacterActionContext.Release);
            MonoBehaviourMaster.Instance.OnFixedUpdate -= Action4Hold;
        }

        private void AttemptAction(ICharacterActionData actionData, CharacterActionContext interaction) {
            OnActionAttempted?.Invoke(actionData, interaction);
        }

        private void OnSkillSet1Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.SkillMode1, CharacterActionContext.Initiate);
            if(CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills1ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnSkillSet1Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += SkillSet1Held;
        }

        private void SkillSet1Held() {
            AttemptAction(CurrentActionSet.SkillMode1, CharacterActionContext.Hold);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills1ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnSkillSet1Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.SkillMode1, CharacterActionContext.Release);
            if (CurrentActionSet == _skills1ActionSet) {
                CurrentActionSet = _normalActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
            MonoBehaviourMaster.Instance.OnFixedUpdate -= SkillSet1Held;
        }

        private void OnSkillSet2Pressed(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.SkillMode2, CharacterActionContext.Initiate);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills2ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnSkillSet2Held(InputAction.CallbackContext callbackContext) {
            MonoBehaviourMaster.Instance.OnFixedUpdate += SkillSet2Held;
        }

        private void SkillSet2Held() {
            AttemptAction(CurrentActionSet.SkillMode2, CharacterActionContext.Hold);
            if (CurrentActionSet == _normalActionSet) {
                CurrentActionSet = _skills2ActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
        }

        private void OnSkillSet2Released(InputAction.CallbackContext callbackContext) {
            AttemptAction(CurrentActionSet.SkillMode2, CharacterActionContext.Release);
            if (CurrentActionSet == _skills2ActionSet) {
                CurrentActionSet = _normalActionSet;
                OnCurrentActionSetUpdated?.Invoke();
            }
            MonoBehaviourMaster.Instance.OnFixedUpdate -= SkillSet2Held;
        }
    }
}
