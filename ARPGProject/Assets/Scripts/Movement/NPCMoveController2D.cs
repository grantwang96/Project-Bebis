using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCMoveController2D : CharacterComponent, IMoveController {

        private const float RotationThreshold = 0.25f;

        public float MoveMagnitude { get; private set; }
        public Vector3 Move => _move;
        public Vector3 Rotation => _rotation;
        public Transform Body => transform;
        public Transform Center => transform;

        public bool IsGrounded => true;

        [SerializeField] private Vector2 _move;
        [SerializeField] private Vector2 _rotation;
        [SerializeField] private Vector2 _forceVector;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _canRotate = true;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _turnLerpSpeed = .8f;
        [SerializeField] private float _linearDrag = 1f;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private NPCNavigator _npcNavigator;

        private bool _overrideMovement;
        private bool _overrideRotation;

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovementInput(_npcNavigator.MoveInput);
            ProcessRotationInput(_npcNavigator.RotationInput);
            ProcessMovement();
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += (Vector2)totalForce;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += (Vector2)direction * force;
        }

        public void OverrideMovement(Vector3 direction) {
            _move = direction;
            _overrideMovement = true;
        }

        public void OverrideRotation(Vector3 direction) {
            _rotation = direction;
            _overrideRotation = true;
        }

        private void ProcessMovementInput(Vector2 moveInput) {
            if (!_canMove || !_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement) || _overrideMovement) {
                _move = new Vector2(0f, 0f);
                return;
            }
            _move = moveInput;
        }

        private void ProcessRotationInput(Vector2 moveInput) {
            if (!IsMoving(_move) || _overrideRotation) {
                return;
            }
            if (Mathf.Abs(_move.x) > RotationThreshold) {
                _rotation.x = ExtraMath.Magnitude(_move.x, 1f);
            } else {
                _rotation.x = 0f;
            }
            if (Mathf.Abs(_move.y) > RotationThreshold) {
                _rotation.y = ExtraMath.Magnitude(_move.y, 1f);
            } else {
                _rotation.y = 0f;
            }
        }

        private bool IsMoving(Vector2 input) {
            return Mathf.Abs(input.x) > RotationThreshold || Mathf.Abs(input.y) > RotationThreshold;
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

        private void ProcessMovement() {
            // move the character controller
            Vector2 moveVector = Move * _moveSpeed;
            moveVector += _forceVector;
            _rigidbody2D.MovePosition(_rigidbody2D.position + moveVector * Time.deltaTime);
            _overrideMovement = false;
        }
    }
}
