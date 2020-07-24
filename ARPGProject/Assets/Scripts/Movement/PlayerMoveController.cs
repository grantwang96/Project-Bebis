using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private float _airImpulse;
        [SerializeField] private float _turnLerpSpeed;
        [SerializeField] private float _linearDrag;
        [SerializeField] private Vector3 _forceVector;

        [SerializeField] private int _movementRestrictions;
        [SerializeField] private bool _rotationRestrictions;

        private void Start() {
            _character.ActionController.OnPerformActionSuccess += OnCharacterPerformAction;
            _character.Damageable.OnHitStun += OnDamageableHitStun;
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

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovementInput(InputController.Instance.MoveInput);
            ProcessRotationInput();
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

        // call in FixedUpdate to avoid false positives
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

        private void ProcessMovementInput(Vector2 moveInput) {
            // if there are movement restrictions or character is in the middle of an action, return
            if (_movementRestrictions > 0 || !CanMove()) {
                return;
            }
            SetMove(moveInput);
            // set the move magnitude based on largest input
            float x = Mathf.Abs(moveInput.x);
            float y = Mathf.Abs(moveInput.y);
            MoveMagnitude = Mathf.Max(x, y);
        }

        private void SetMove(Vector2 moveInput) {
            float multiplier = 1f;
            // if character is grounded
            if (IsGrounded) {
                // reset the direction
                _move = new Vector3(0f, _move.y, 0f);
            } else {
                multiplier = _airImpulse;
            }
            // set the direction based on camera orientation
            _move += CameraController.Instance.CameraRoot.right * moveInput.x * multiplier;
            _move += CameraController.Instance.CameraRoot.forward * moveInput.y * multiplier;
            // clamp at magnitude 1
            _move.x = Mathf.Clamp(_move.x, -1f, 1f);
            _move.z = Mathf.Clamp(_move.z, -1f, 1f);
        }

        private bool CanMove() {
            return _character.ActionController.Permissions.HasFlag(ActionPermissions.Movement);
        }

        private void ProcessRotationInput() {
            if (!IsMoving(_move) || !CanRotate()) {
                return;
            }
            _rotation = _move;
        }
        
        // can this character change rotation
        private bool CanRotate() {
            ActionPermissions permissions = _character.ActionController.Permissions;
            return permissions.HasFlag(ActionPermissions.Rotation);
        }

        // is this character moving
        private bool IsMoving(Vector3 input) {
            return !Mathf.Approximately(input.x, 0f) || !Mathf.Approximately(input.z, 0f);
        }

        // is this character grounded
        private bool IsCharacterGrounded() {
            return _characterController.isGrounded;
        }

        private void ProcessMovement() {
            // move the character controller
            float speed = IsGrounded ? _moveSpeed : _airSpeed;
            Vector3 moveVector = Move * speed;
            moveVector += _forceVector;
            _characterController.Move(moveVector * Time.deltaTime);
        }

        private void ProcessRotation() {
            if (_bodyRoot.forward == Rotation) {
                return;
            }
            _bodyRoot.forward = Vector3.RotateTowards(_bodyRoot.forward, Rotation, _turnLerpSpeed * Time.deltaTime, 0f);
        }

        // when the character performs an action
        private void OnCharacterPerformAction() {
            if (!CanMove() && IsGrounded) {
                _move = new Vector3(0f, _move.y, 0f);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (!CanMove() && IsGrounded) {
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
