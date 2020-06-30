using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class CameraController : MonoBehaviour {

        public static CameraController Instance { get; private set; }

        public Transform CameraRoot { get; private set; }

        [SerializeField] private Transform _playerCharacter;
        [SerializeField] private Transform _cameraPivotX;
        [SerializeField] private Transform _cameraPivotY;

        [SerializeField] private float _cameraXUpperLimit;
        [SerializeField] private float _cameraXLowerLimit;
        [SerializeField] private float _cameraTurnSpeed;

        private void Awake() {
            Instance = this;
            CameraRoot = _cameraPivotX;
        }

        private void FixedUpdate() {
            ProcessRotation();
        }

        private void LateUpdate() {
            FollowCharacter();
        }

        private void FollowCharacter() {
            _cameraPivotX.position = _playerCharacter.position;
        }

        private void ProcessRotation() {
            Vector2 lookInput = InputController.Instance.LookInput;
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
