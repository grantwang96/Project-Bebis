using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Bebis
{
    public class PlayerControllerV2 : IUnitController
    {
        // input ids 
        private const string MoveInputId = "Move";
        private const string LookInputId = "Look";

        public static PlayerControllerV2 Instance { get; private set; }

        public ITargetManagerV2 TargetManager => _playerTargetManager;
        public ICharacterData CharacterData { get; } // TODO: point this to 
        public Vector3 MovementInput { get; private set; }
        public Vector3 RotationInput { get; private set; }
        public Transform PlayerTransform => _playerCharacter.GameObject.transform;

        public event Action<ICharacterActionDataV2, CharacterActionContext> OnActionAttempted;
        public event Action OnRegisteredCharacterActionsUpdated;

        private ICharacterV2 _playerCharacter;
        private PlayerActionInfoProviderV2 _playerActionInfoProvider;
        private PlayerTargetManagerV2 _playerTargetManager;
        private readonly Dictionary<string, ICharacterActionDataV2> _registeredActions = new Dictionary<string, ICharacterActionDataV2>();

        public PlayerControllerV2() {
            Instance = this;
        }

        public void Initialize(ICharacterV2 character) {
            _playerCharacter = character;
            _playerActionInfoProvider = new PlayerActionInfoProviderV2(_playerCharacter);
            _playerActionInfoProvider.OnActionAttempted += OnActionInputted;
            _playerTargetManager = new PlayerTargetManagerV2();
            _playerTargetManager.Initialize(_playerCharacter);
            PlayerCharacterManager.Instance.OnPlayerSkillsLoadoutV2Set += OnPlayerSkillsLoadoutV2Set;
            OnPlayerSkillsLoadoutV2Set(PlayerCharacterManager.Instance.SkillsLoadoutV2);
            MonoBehaviourMaster.Instance.OnUpdate += OnUpdate;
        }

        private void OnUpdate() {
            ProcessInputs();
        }

        private void ProcessInputs() {
            if (!InputController.Instance.PlayerInputActionMap.enabled) {
                return;
            }
            ProcessMovementInput();
            ProcessRotationInput();
        }

        private void ProcessMovementInput() {
            // read inputs from the input system
            Vector3 moveInput = InputController.Instance.PlayerInputActionMap[MoveInputId].ReadValue<Vector2>();
            // build world space movement input using the camera's transform
            Vector3 nextMovementInput = CameraController.Instance.CameraRoot.right * moveInput.x;
            nextMovementInput += CameraController.Instance.CameraRoot.forward * moveInput.y;
            MovementInput = nextMovementInput;
        }

        private void ProcessRotationInput() {
            // TODO: have context sensitive rotation input handling. For now, simply use movement input
            RotationInput = MovementInput;
        }

        private void OnActionInputted(string actionId, CharacterActionContext context) {
            if(!_registeredActions.TryGetValue(actionId, out ICharacterActionDataV2 data)) {
                Debug.LogError($"[{_playerCharacter.GameObject.name}]: Action with id '{actionId}' not found in registered actions!");
                return;
            }
            OnActionAttempted?.Invoke(data, context);
        }

        private void OnPlayerSkillsLoadoutV2Set(IPlayerSkillsLoadoutV2 loadout) {
            _playerActionInfoProvider.UpdateLoadout(loadout);
            _registeredActions.Clear();
            TryAddLoadoutAction(loadout.NormalAttackSkill);
            TryAddLoadoutAction(loadout.NormalAerialAttackSkill);
            TryAddLoadoutAction(loadout.NormalBtn1Skill);
            TryAddLoadoutAction(loadout.NormalBtn4Skill);
            TryAddLoadoutAction(loadout.SecondaryAttackSkill);
            TryAddLoadoutAction(loadout.SecondaryAerialAttackSkill);
            TryAddLoadoutAction(loadout.RightTriggerSkill);
            TryAddLoadoutAction(loadout.LeftTriggerSkill);
            TryAddLoadoutAction(loadout.SkillSet1Btn1Skill);
            TryAddLoadoutAction(loadout.SkillSet1Btn2Skill);
            TryAddLoadoutAction(loadout.SkillSet1Btn3Skill);
            TryAddLoadoutAction(loadout.SkillSet1Btn4Skill);
            TryAddLoadoutAction(loadout.SkillSet2Btn1Skill);
            TryAddLoadoutAction(loadout.SkillSet2Btn2Skill);
            TryAddLoadoutAction(loadout.SkillSet2Btn3Skill);
            TryAddLoadoutAction(loadout.SkillSet2Btn4Skill);
            OnRegisteredCharacterActionsUpdated?.Invoke();
        }

        private void TryAddLoadoutAction(ICharacterActionDataV2 data) {
            if (data == null) {
                return;
            }
            _registeredActions.Add(data.Id, data);
        }
    }
}
