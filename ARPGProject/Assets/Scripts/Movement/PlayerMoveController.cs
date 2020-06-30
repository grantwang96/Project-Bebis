using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerMoveController : CharacterComponent, IMoveController {

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

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _turnLerpSpeed = .8f;
        [SerializeField] private float _linearDrag = 1f;
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
            IsGrounded = _characterController.isGrounded;
        }

        private void ProcessExternalForces() {
            ProcessGravity();
            ProcessDrag();
        }

        private void ProcessGravity() {
            if (_forceVector.y > Physics.gravity.y) {
                _forceVector.y += Physics.gravity.y * Time.deltaTime;
            }
        }

        private void ProcessDrag() {
            if(Mathf.Approximately(_forceVector.x, 0f) && Mathf.Approximately(_forceVector.z, 0f)) {
                return;
            }
            if (_characterController.isGrounded) {
                _forceVector.x = ExtraMath.Lerp(_forceVector.x, 0f, _linearDrag * Time.deltaTime);
                _forceVector.z = ExtraMath.Lerp(_forceVector.z, 0f, _linearDrag * Time.deltaTime);
            }
        }

        private void ProcessMovementInput(Vector2 moveInput) {
            if (_movementRestrictions > 0 || !_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement)) {
                return;
            }
            if (!_characterController.isGrounded) {
                return;
            }
            _move = new Vector3(0f, _move.y, 0f);
            _move += CameraController.Instance.CameraRoot.right * moveInput.x;
            _move += CameraController.Instance.CameraRoot.forward * moveInput.y;
        }

        private void ProcessRotationInput() {
            if (!IsMoving(_move)) {
                return;
            }
            _rotation = _move;
        }

        private bool IsMoving(Vector3 input) {
            return !Mathf.Approximately(input.x, 0f) || !Mathf.Approximately(input.z, 0f);
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

        private void OnCharacterPerformAction() {
            ActionPermissions permissions = _character.ActionController.Permissions;
            if (!permissions.HasFlag(ActionPermissions.Movement) && IsGrounded) {
                _move = new Vector3(0f, _move.y, 0f);
            }
            if (!permissions.HasFlag(ActionPermissions.Rotation)) {
                // todo: process rotation things
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (!_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement) && IsGrounded) {
                _move = new Vector3(0f, _move.y, 0f);
            }
        }

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
