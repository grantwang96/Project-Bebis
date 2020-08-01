﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bebis {
    public class PlayerMoveController : CharacterComponent, IMoveController {

        public float MoveMagnitude { get; private set; }
        public Vector3 Move => _move;
        public Vector3 Rotation => _rotation;
        public Transform Body => _bodyRoot;
        public Transform Center => _bodyCenter;

        [SerializeField] private Vector3 _move;
        [SerializeField] private Vector3 _rotation;

        public bool IsGrounded { get; private set; }

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _bodyCenter;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _airSpeed;
        [SerializeField] private float _turnLerpSpeed;
        [SerializeField] private float _linearDrag;
        [SerializeField] private Vector3 _forceVector;

        [SerializeField] private int _movementRestrictions;
        [SerializeField] private bool _rotationRestrictions;

        private InputAction _moveInputAction;
        private bool _overrideMovement;
        private bool _overrideRotation;

        private void Start() {
            _character.ActionController.OnPerformActionSuccess += OnCharacterPerformAction;
            _character.Damageable.OnHitStun += OnDamageableHitStun;
            _moveInputAction = InputController.Instance.PlayerInputActionMap["Move"];
        }

        private void OnDestroy() {
            _character.ActionController.OnPerformActionSuccess -= OnCharacterPerformAction;
            _character.Damageable.OnHitStun -= OnDamageableHitStun;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += direction * force;
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            if (overrideForce) {
                _forceVector = Vector3.zero;
            }
            _forceVector += totalForce;
        }

        public void OverrideRotation(Vector3 direction) {
            _rotation = direction;
            _overrideRotation = true;
        }

        public void OverrideMovement(Vector3 direction) {
            _move = direction;
            _overrideMovement = true;
        }

        private void Update() {
            HandleMoveInput(_moveInputAction.ReadValue<Vector2>());
            HandleRotation();
        }

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovement();
            ProcessRotation();
            LateSetIsGrounded();
        }

        // process forces like gravity and other applied forces
        private void ProcessExternalForces() {
            ProcessGravity();
            ProcessDrag();
        }

        // apply gravity to downward velocity each frame as needed
        private void ProcessGravity() {
            if (_forceVector.y > Physics.gravity.y) {
                _forceVector.y += Physics.gravity.y * Time.deltaTime;
            }
        }

        // call in FixedUpdate at the end to avoid false positives
        private void LateSetIsGrounded() {
            IsGrounded = IsCharacterGrounded();
        }

        // handles ground friction when an active force is applied to a character
        private void ProcessDrag() {
            if (Mathf.Approximately(_forceVector.x, 0f) && Mathf.Approximately(_forceVector.z, 0f)) {
                return;
            }
            if (_characterController.isGrounded) {
                _forceVector.x = ExtraMath.Lerp(_forceVector.x, 0f, _linearDrag * Time.deltaTime);
                _forceVector.z = ExtraMath.Lerp(_forceVector.z, 0f, _linearDrag * Time.deltaTime);
            }
        }

        private void HandleMoveInput(Vector2 moveInput) {
            // if the character can take move inputs
            if (!CanInputMove()) {
                return;
            }
            // set move magnitude
            MoveMagnitude = Mathf.Clamp01(moveInput.magnitude);
            // set move vector
            Vector2 inputNormalized = moveInput.normalized;
            Vector3 localizedInput = new Vector3();
            localizedInput += CameraController.Instance.CameraRoot.right * inputNormalized.x;
            localizedInput += CameraController.Instance.CameraRoot.forward * inputNormalized.y;
            if (IsGrounded) {
                // directly set the localized input
                _move = localizedInput;
            } else {
                // lerp the move vector towards localized input
                _move.x = ExtraMath.Lerp(_move.x, localizedInput.x, 0.05f);
                _move.z = ExtraMath.Lerp(_move.z, localizedInput.z, 0.05f);
            }
        }

        private bool CanInputMove() {
            return _movementRestrictions <= 0 && 
                _character.ActionController.Permissions.HasFlag(ActionPermissions.Movement) &&
                !_overrideMovement;
        }

        private void HandleRotation() {
            // if the character can take rotation inputs (built from move input)
            if (CanRotate()) {
                _rotation = _move;
            }
        }
        
        // can this character change rotation
        private bool CanRotate() {
            ActionPermissions permissions = _character.ActionController.Permissions;
            return permissions.HasFlag(ActionPermissions.Rotation) &&
                IsMoving(_move) &&
                !_overrideRotation;
        }

        // is this character moving
        private static bool IsMoving(Vector2 input) {
            return !Mathf.Approximately(input.x, 0f) || !Mathf.Approximately(input.y, 0f);
        }

        // is this character grounded
        private bool IsCharacterGrounded() {
            return _characterController.isGrounded;
        }

        private void ProcessMovement() {
            // move the character controller
            float speed = IsGrounded ? _moveSpeed : _airSpeed;
            Vector3 moveVector = Move * speed * MoveMagnitude;
            moveVector += _forceVector;
            _characterController.Move(moveVector * Time.deltaTime);
            _overrideMovement = false;
        }

        private void ProcessRotation() {
            if (_bodyRoot.forward == Rotation) {
                return;
            }
            _bodyRoot.forward = Vector3.RotateTowards(_bodyRoot.forward, Rotation, _turnLerpSpeed * Time.deltaTime, 0f);
            _overrideRotation = false;
        }

        // when the character performs an action
        private void OnCharacterPerformAction() {
            if (!CanInputMove() && IsGrounded) {
                _move = new Vector3(0f, _move.y, 0f);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (!CanInputMove() && IsGrounded) {
                _move = new Vector3(0f, _move.y, 0f);
            }
        }

        // upon entering hitstun
        private void OnDamageableHitStun(HitEventInfo hitEventInfo) {
            _movementRestrictions++;
            _move = new Vector3(0f, _move.y, 0f);
            _character.AnimationController.OnAnimationStateUpdated += OnDamageableAnimationCompleted;
        }

        private void OnDamageableAnimationCompleted(AnimationState state) {
            if (state != AnimationState.Completed) {
                return;
            }
            _character.AnimationController.OnAnimationStateUpdated -= OnDamageableAnimationCompleted;
            _movementRestrictions = Mathf.Max(0, _movementRestrictions - 1);
        }
    }
}
