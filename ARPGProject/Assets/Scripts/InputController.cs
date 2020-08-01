using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Bebis {
    public class InputController : MonoBehaviour {

        public enum ControlState {
            Gameplay,
            UI
        }

        public static InputController Instance { get; private set; }
        
        [SerializeField] private InputActionAsset _gameplayInputActionAsset; // the normal gameplay input action asset
        
        public InputActionMap PlayerInputActionMap { get; private set; }

        public event Action<ControlState> OnControlStatusUpdated;

        public void EnterControlState(ControlState status) {
            OnControlStatusUpdated?.Invoke(status);
        }

        private void Awake() {
            Instance = this;
            PlatformSetup();

            PlayerInputActionMap = _gameplayInputActionAsset.FindActionMap("Player");
        }
        
        private void PlatformSetup() {
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    SetupMouse();
                    break;
                default:
                    break;
            }
        }
        private void SetupMouse() {
            // temp
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
