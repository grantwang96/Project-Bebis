using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Bebis {
    public class PlayerMoveController : CharacterComponent, IMoveController {

        private const string LandStartId = "LANDSTART";
        private const string LandEndId = "LANDEND";

        public float MoveMagnitude { get; private set; }
        public Vector3 Move => _move;
        public Vector3 Rotation => _rotation;
        public Transform Body => _bodyRoot;
        public Transform Center => _bodyCenter;
        public bool IsGrounded { get; private set; }
        public float Height { get; private set; }
        public RestrictionController MoveRestrictions { get; } = new RestrictionController();
        public RestrictionController LookRestrictions { get; } = new RestrictionController();

        public event Action OnLanding;

        [SerializeField] private Vector3 _move;
        [SerializeField] private Vector3 _rotation;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _bodyCenter;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _airSpeed;
        [SerializeField] private float _turnLerpSpeed;
        [SerializeField] private float _linearDrag;
        [SerializeField] private Vector3 _currentVelocity;

        private bool _overrideMovement;
        private bool _overrideRotation;

        private IMoveControllerInfoProvider _moveInfoProvider;

        public void Initialize(IMoveControllerInfoProvider moveInfoProvider) {
            _moveInfoProvider = moveInfoProvider;
            _character.Damageable.OnHitStun += OnDamageableHitStun;
            _character.AnimationController.OnAnimationMessageSent += OnAnimationMessageSent;

            Height = _characterController.height;
        }

        private void OnDestroy() {
            _character.Damageable.OnHitStun -= OnDamageableHitStun;
            _character.AnimationController.OnAnimationMessageSent -= OnAnimationMessageSent;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _currentVelocity = Vector3.zero;
            }
            _currentVelocity += direction * force;
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            if (overrideForce) {
                _currentVelocity = Vector3.zero;
            }
            _currentVelocity += totalForce;
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
            HandleMoveInput();
            HandleRotation();
        }

        private void FixedUpdate() {
            ProcessExternalForces();
            ProcessMovement();
            ProcessRotation();
            LateCheckGrounded();
        }

        // process forces like gravity and other applied forces
        private void ProcessExternalForces() {
            ProcessGravity();
            ProcessDrag();
        }

        // apply gravity to downward velocity each frame as needed
        private void ProcessGravity() {
            if (_currentVelocity.y > Physics.gravity.y) {
                _currentVelocity.y += Physics.gravity.y * Time.deltaTime;
            }
        }

        // call in FixedUpdate at the end to avoid false positives
        private void LateCheckGrounded() {
            if (IsGrounded && !IsCharacterGrounded()) {
                IsGrounded = false;
            }
        }

        // handles ground friction when an active force is applied to a character
        private void ProcessDrag() {
            if (Mathf.Approximately(_currentVelocity.x, 0f) && Mathf.Approximately(_currentVelocity.z, 0f)) {
                return;
            }
            if (_characterController.isGrounded) {
                _currentVelocity.x = ExtraMath.Lerp(_currentVelocity.x, 0f, _linearDrag * Time.deltaTime);
                _currentVelocity.z = ExtraMath.Lerp(_currentVelocity.z, 0f, _linearDrag * Time.deltaTime);
            }
        }

        private void HandleMoveInput() {
            Vector2 moveInput = _moveInfoProvider.IntendedMoveDirection;
            // set move vector
            Vector2 inputNormalized = moveInput.normalized;
            Vector3 localizedInput = new Vector3();
            localizedInput += CameraController.Instance.CameraRoot.right * inputNormalized.x;
            localizedInput += CameraController.Instance.CameraRoot.forward * inputNormalized.y;
            _move = localizedInput;
            // set move magnitude
            MoveMagnitude = Mathf.Clamp01(_move.magnitude);
        }

        private bool CanMove() {
            return !MoveRestrictions.Restricted &&
                !_overrideMovement;
        }

        private void HandleRotation() {
            // if the character can take rotation inputs (built from move input)
            if (CanRotate()) {
                _rotation = IsMoving(_move) ? _move : _rotation;
            }
        }
        
        // can this character change rotation
        private bool CanRotate() {
            return IsGrounded &&
                !LookRestrictions.Restricted &&
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
            Vector3 moveVector = CalculateMoveFromInput();
            // if the character can take move inputs
            moveVector += _currentVelocity;
            _characterController.Move(moveVector * Time.deltaTime);
            _overrideMovement = false;
        }

        private Vector3 CalculateMoveFromInput() {
            float speed = IsGrounded ? _moveSpeed : _airSpeed;
            Vector3 moveVector = new Vector3();
            if (CanMove()) {
                Vector3 fullMove = Move * speed * MoveMagnitude;
                if (IsGrounded) {
                    moveVector = fullMove;
                } else {
                    Vector3 nextVelocity = _currentVelocity + (fullMove * Time.deltaTime);
                    if (nextVelocity.magnitude <= _airSpeed) {
                        _currentVelocity = nextVelocity;
                    }
                }
            }
            return moveVector;
        }

        private void ProcessRotation() {
            if (_bodyRoot.forward == Rotation) {
                return;
            }
            _bodyRoot.forward = Vector3.RotateTowards(_bodyRoot.forward, Rotation, _turnLerpSpeed * Time.deltaTime, 0f);
            _overrideRotation = false;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (!CanMove() && _characterController.collisionFlags == CollisionFlags.Sides) {
                _currentVelocity = new Vector3(0f, _move.y, 0f);
            }
            if (_characterController.collisionFlags.HasFlag(CollisionFlags.Below) && !IsGrounded) {
                IsGrounded = true;
            }
        }
        
        // listen for animation updates when landing
        private void OnAnimationMessageSent(string state) {
            if (state == LandStartId) {
                MoveRestrictions.AddRestriction("Landing");
                _currentVelocity = new Vector3(0f, _currentVelocity.y, 0f);
                _move = Vector3.zero;
                OnLanding?.Invoke();
            } else if(state == LandEndId) {
                MoveRestrictions.RemoveRestriction("Landing");
            }
        }

        // upon entering hitstun
        private void OnDamageableHitStun(HitEventInfo hitEventInfo) {
            MoveRestrictions.RemoveRestriction("TakeDamage");
            MoveRestrictions.RemoveRestriction("TakeDamage");
            MoveRestrictions.AddRestriction("TakeDamage");
            LookRestrictions.AddRestriction("TakeDamage");
            _move = new Vector3(0f, _move.y, 0f);
            _character.AnimationController.OnAnimationStateUpdated += OnDamageableAnimationCompleted;
        }

        private void OnDamageableAnimationCompleted(AnimationState state) {
            if (state != AnimationState.Completed) {
                return;
            }
            _character.AnimationController.OnAnimationStateUpdated -= OnDamageableAnimationCompleted;
            MoveRestrictions.RemoveRestriction("TakeDamage");
            LookRestrictions.RemoveRestriction("TakeDamage");
        }
    }
}
