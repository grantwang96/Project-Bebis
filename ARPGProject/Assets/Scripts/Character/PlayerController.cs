using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Winston;

namespace Bebis {
    public class PlayerController : MonoBehaviour, IMoveControllerInfoProvider, IActionControllerInfoProvider {

        private const string KeyBoardMouse = "Keyboard&Mouse";
        private const string PlayerHudUIId = "PlayerHud";

        // move controller implementations
        public Vector3 IntendedMoveDirection => _intendedMoveDirection;
        public Vector3 IntendedLookDirection => _intendedLookDirection;

        // action controller implementations
        public event Action<ICharacterActionData, CharacterActionContext> OnActionAttempted;

        // input ids 
        [SerializeField] private string _moveInputId;
        [SerializeField] private string _lookInputId;
        [SerializeField] private Vector3 _intendedMoveDirection;
        [SerializeField] private Vector3 _intendedLookDirection;
        [SerializeField] private PlayerCharacter _playerCharacter;
        
        private PlayerActionInfoProvider _playerActionInfoProvider;
        private IUIDisplay _playerHudDisplay;

        // Start is called before the first frame update
        private void Start() {
            CreateActionInfoProviderInternal();
            SubscribeToEvents();
            CreatePlayerHud();
            SetPlayerHudEnabled(true);
        }

        private void OnDestroy() {
            UnsubscribeToEvents();
        }

        // Update is called once per frame
        private void Update() {
            ProcessInput();
        }
        
        private void SubscribeToEvents() {
            _playerActionInfoProvider.OnActionAttempted += ActionAttempted;
        }

        private void CreateActionInfoProviderInternal() {
            _playerActionInfoProvider = new PlayerActionInfoProvider(_playerCharacter);
        }

        private void UnsubscribeToEvents() {
            _playerActionInfoProvider.OnActionAttempted -= ActionAttempted;
        }

        private void CreatePlayerHud() {
            _playerHudDisplay = ManagerMaster.UIManager.CreateUIDisplay(PlayerHudUIId, UILayerId.Background);
            if(_playerHudDisplay == null) {
                Debug.LogError($"[{nameof(PlayerController)}]: Could not create player hud display with id \"{PlayerHudUIId}\"!");
                return;
            }
            _playerHudDisplay.Initialize(new PlayerHudInitializationData() {
                PlayerActionInfoProvider = _playerActionInfoProvider
            });
        }

        private void SetPlayerHudEnabled(bool enabled) {

        }

        private void ProcessInput() {
            if (!InputController.Instance.PlayerInputActionMap.enabled) {
                return;
            }
            // read inputs from the input system
            _intendedMoveDirection = InputController.Instance.PlayerInputActionMap[_moveInputId].ReadValue<Vector2>();
            _intendedLookDirection = InputController.Instance.PlayerInputActionMap[_lookInputId].ReadValue<Vector2>();
        }

        private void ActionAttempted(ICharacterActionData data, CharacterActionContext context) {
            OnActionAttempted?.Invoke(data, context);
        }
    }
}
