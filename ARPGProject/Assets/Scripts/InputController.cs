using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class InputController : MonoBehaviour {

        public enum ControlState {
            Gameplay,
            UI
        }

        private const string MoveXAxisId = "MoveXAxis";
        private const string MoveYAxisId = "MoveYAxis";
        private const string LookXAxisId = "LookXAxis";
        private const string LookYAxisId = "LookYAxis";

        private const string Button1Id = "Button1";
        private const string Button2Id = "Button2";
        private const string Button3Id = "Button3";
        private const string Button4Id = "Button4";

        public static InputController Instance { get; private set; }

        [SerializeField] private Vector2 _moveInput = new Vector2(0, 0);
        public Vector2 MoveInput => _moveInput;
        [SerializeField] private Vector2 _lookInput = new Vector2(0, 0);
        public Vector2 LookInput => _lookInput;

        public event Action<ControlState> OnControlStatusUpdated;

        public event Action OnBtn1Pressed;
        public event Action OnBtn1Held;
        public event Action OnBtn1Released;

        public event Action OnBtn2Pressed;
        public event Action OnBtn2Held;
        public event Action OnBtn2Released;

        public event Action OnBtn3Pressed;
        public event Action OnBtn3Held;
        public event Action OnBtn3Released;

        public event Action OnBtn4Pressed;
        public event Action OnBtn4Held;
        public event Action OnBtn4Released;

        public void EnterControlState(ControlState status) {
            OnControlStatusUpdated?.Invoke(status);
        }

        private void Awake() {
            Instance = this;
            PlatformSetup();
        }

        private void Update() {
            AxisInputs();
            ButtonInputs();
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

        private void AxisInputs() {
            _moveInput.x = Input.GetAxis(MoveXAxisId);
            _moveInput.y = Input.GetAxis(MoveYAxisId);
            _lookInput.x = Input.GetAxis(LookXAxisId);
            _lookInput.y = Input.GetAxis(LookYAxisId);
        }

        private void ButtonInputs() {
            Button1Inputs();
            Button2Inputs();
            Button3Inputs();
            Button4Inputs();
        }

        private void Button1Inputs() {
            if (Input.GetButtonDown(Button1Id)) {
                OnBtn1Pressed?.Invoke();
            } else if (Input.GetButton(Button1Id)) {
                OnBtn1Held?.Invoke();
            } else if (Input.GetButtonUp(Button1Id)) {
                OnBtn1Released?.Invoke();
            }
        }

        private void Button2Inputs() {
            if (Input.GetButtonDown(Button2Id)) {
                OnBtn2Pressed?.Invoke();
            } else if (Input.GetButton(Button2Id)) {
                OnBtn2Held?.Invoke();
            } else if (Input.GetButtonUp(Button2Id)) {
                OnBtn2Released?.Invoke();
            }
        }

        private void Button3Inputs() {
            if (Input.GetButtonDown(Button3Id)) {
                OnBtn3Pressed?.Invoke();
            } else if (Input.GetButton(Button3Id)) {
                OnBtn3Held?.Invoke();
            } else if (Input.GetButtonUp(Button3Id)) {
                OnBtn3Released?.Invoke();
            }
        }

        private void Button4Inputs() {
            if (Input.GetButtonDown(Button4Id)) {
                OnBtn4Pressed?.Invoke();
            } else if (Input.GetButton(Button4Id)) {
                OnBtn4Held?.Invoke();
            } else if (Input.GetButtonUp(Button4Id)) {
                OnBtn4Released?.Invoke();
            }
        }
    }
}
