using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerMoveControllerV2 : CharacterComponent, IMoveController {

        private const float RotationThreshold = 0.25f;

        public Vector3 Move => _move;
        public Vector3 Rotation => _rotation;
        public Transform Body => transform;

        public bool CanJump => true;

        [SerializeField] private Vector2 _move;
        [SerializeField] private Vector2 _rotation;
        [SerializeField] private Vector2 _forceVector;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _canRotate = true;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _turnLerpSpeed = .8f;
        [SerializeField] private float _linearDrag = 1f;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += (Vector2)direction * force;
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += (Vector2)totalForce;
        }

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovementInput(InputController.Instance.MoveInput);
            ProcessRotationInput(InputController.Instance.MoveInput);
            ProcessMovement();
            // ProcessRotation();
        }

        private void ProcessExternalForces() {
            ProcessDrag();
        }

        private void ProcessDrag() {
            if (Mathf.Approximately(_forceVector.x, 0f) && Mathf.Approximately(_forceVector.y, 0f)) {
                return;
            }
            _forceVector.x = ExtraMath.Lerp(_forceVector.x, 0f, _linearDrag * Time.deltaTime);
            _forceVector.y = ExtraMath.Lerp(_forceVector.y, 0f, _linearDrag * Time.deltaTime);
        }

        private void ProcessMovementInput(Vector2 moveInput) {
            if (!_canMove || !_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement)) {
                _move = new Vector2(0f, 0f);
                return;
            }
            _move = moveInput;
        }

        private void ProcessRotationInput(Vector2 moveInput) {
            if (!IsMoving(_move)) {
                return;
            }
            if(Mathf.Abs(_move.x) > RotationThreshold) {
                _rotation.x = ExtraMath.Magnitude(_move.x, 1f);
            } else {
                _rotation.x = 0f;
            }
            if(Mathf.Abs(_move.y) > RotationThreshold) {
                _rotation.y = ExtraMath.Magnitude(_move.y, 1f);
            } else {
                _rotation.y = 0f;
            }
        }

        private bool IsMoving(Vector2 input) {
            return Mathf.Abs(input.x) > RotationThreshold || Mathf.Abs(input.y) > RotationThreshold;
        }

        private void ProcessMovement() {
            // move the character controller
            Vector2 moveVector = Move * _moveSpeed;
            moveVector += _forceVector;
            _rigidbody2D.MovePosition(_rigidbody2D.position + moveVector * Time.deltaTime);
        }
    }
}
