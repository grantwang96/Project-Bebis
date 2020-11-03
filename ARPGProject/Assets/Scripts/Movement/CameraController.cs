using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bebis {
    public class CameraController : MonoBehaviour {

        public static CameraController Instance { get; private set; }

        public Transform CameraRoot { get; private set; }
        public Camera MainCamera => _mainCamera;

        private ICharacter _playerCharacter;

        [SerializeField] private Transform _cameraPivotX;
        [SerializeField] private Transform _cameraPivotY;
        [SerializeField] private Camera _mainCamera;

        [SerializeField] private float _cameraXUpperLimit;
        [SerializeField] private float _cameraXLowerLimit;
        [SerializeField] private float _cameraTurnSpeed;

        private InputAction _lookInputAction;

        private void Awake() {
            Instance = this;
            CameraRoot = _cameraPivotX;
        }

        private void Start() {
            _playerCharacter = PlayerCharacter.Instance;
            _lookInputAction = InputController.Instance.PlayerInputActionMap["Look"];
        }

        private void FixedUpdate() {
            HandleLookInput(_lookInputAction.ReadValue<Vector2>());
        }

        private void LateUpdate() {
            FollowCharacter();
        }

        private void FollowCharacter() {
            _cameraPivotX.position = _playerCharacter.MoveController.Body.position;
        }

        private void HandleLookInput(Vector2 lookInput) {
            Vector3 cameraX = _cameraPivotX.localEulerAngles;
            cameraX.y += lookInput.x * _cameraTurnSpeed * Time.deltaTime;
            _cameraPivotX.localEulerAngles = cameraX;

            Vector3 cameraY = _cameraPivotY.localEulerAngles;
            cameraY.x -= lookInput.y * _cameraTurnSpeed * Time.deltaTime;
            cameraY.x = ClampXRotation(cameraY.x);
            _cameraPivotY.localEulerAngles = cameraY;
        }

        private float ClampXRotation(float angle) {
            float newValue = angle;
            if(newValue > _cameraXUpperLimit && newValue < _cameraXLowerLimit) {
                if(newValue < 180f) { newValue = _cameraXUpperLimit; }
                else { newValue = _cameraXLowerLimit; }
            }
            return newValue;
        }
    }
}
