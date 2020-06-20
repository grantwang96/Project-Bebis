﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCMoveController3D : CharacterComponent, IMoveController {

        private const float RotationThreshold = 0.25f;

        public Vector3 Move => _move;
        public Vector3 Rotation => _rotation;
        public Transform Body => transform;

        public bool IsGrounded => true;

        [SerializeField] private Vector3 _move;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private Vector3 _forceVector;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _canRotate = true;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _turnLerpSpeed = .8f;
        [SerializeField] private float _linearDrag = 1f;
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private NPCNavigator _npcNavigator;

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovementInput(_npcNavigator.MoveInput);
            ProcessRotationInput(_npcNavigator.RotationInput);
            ProcessMovement();
            ProcessRotation();
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += totalForce;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += direction * force;
        }

        private void ProcessMovementInput(Vector3 moveInput) {
            if (!_canMove || !_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement)) {
                _move = new Vector3(0f, 0f);
                return;
            }
            _move = moveInput;
        }

        private void ProcessRotationInput(Vector3 moveInput) {
            if (!IsMoving(_move)) {
                return;
            }
            _rotation = moveInput;
        }

        private bool IsMoving(Vector3 input) {
            return Mathf.Abs(input.x) > RotationThreshold || Mathf.Abs(input.z) > RotationThreshold;
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
            Vector3 moveVector = Move * _moveSpeed;
            moveVector += _forceVector;
            _characterController.Move(moveVector * Time.deltaTime);
        }

        private void ProcessRotation() {
            if (_bodyRoot.forward == Rotation) {
                return;
            }
            _bodyRoot.forward = Vector3.RotateTowards(_bodyRoot.forward, _move, _turnLerpSpeed * Time.deltaTime, 0f);
        }
    }
}
