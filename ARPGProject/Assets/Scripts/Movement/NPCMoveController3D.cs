using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Bebis {
    public class NPCMoveController3D : CharacterComponent, IMoveController {

        private const float RotationThreshold = 0.25f;

        public float MoveMagnitude { get; private set; }
        public Vector3 Move { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Transform Body => _bodyRoot;
        public Transform Center => _bodyCenter;
        public bool IsGrounded => IsCharacterGrounded();
        public float Height => _characterController.height;

        public NavMeshPathStatus PathStatus => _navMeshAgent.path?.status ?? NavMeshPathStatus.PathInvalid;
        
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _canRotate = true;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _linearDrag = 1f;
        [SerializeField] private float _weight;

        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _bodyCenter;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Collider _nonPhysicsModeCollider;

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        private bool _overrideMovement;
        private bool _overrideRotation;
        private float _currentSpeed;
        private CharacterMoveMode _currentMoveMode;
        [SerializeField] private bool _physicsMode;
        private bool _pathing;
        private Vector3 _totalExternalForces;

        public event Action<bool> OnArriveDestination;

        private void FixedUpdate() {
            SetMoveControllerValues();
            CheckArrivedDestination();
            ProcessExternalForces();
            CheckIsGrounded();
        }

        private void CheckIsGrounded() {
            if (!IsGrounded && !_physicsMode) {
                SetPhysicsMode(true);
            }
        }

        private void CheckArrivedDestination() {
            if (!_pathing || _physicsMode) {
                return;
            }
            float distance = Vector3.Distance(_navMeshAgent.nextPosition, _npcNavigator.TargetPosition);
            if(distance <= _navMeshAgent.stoppingDistance) {
                _pathing = false;
                ClearDestination();
                OnArriveDestination?.Invoke(true);
            }
        }

        private void SetPhysicsMode(bool mode) {
            _physicsMode = mode;
            _characterController.enabled = _physicsMode;
            _nonPhysicsModeCollider.enabled = !_physicsMode;
            _navMeshAgent.Warp(transform.position);
            _navMeshAgent.updatePosition = !_physicsMode;
            _navMeshAgent.updateRotation = !_physicsMode;
        }

        private void ProcessExternalForces() {
            if (!_physicsMode) {
                return;
            }
            ProcessDrag();
            ProcessGravity();
            CheckExternalForces();
            if (_characterController.enabled) {
                _characterController.Move(_totalExternalForces * Time.deltaTime);
            }
        }

        private void ProcessDrag() {
            if (IsGrounded) {
                _totalExternalForces.x = ExtraMath.Lerp(_totalExternalForces.x, 0f, _linearDrag * Time.deltaTime);
                _totalExternalForces.z = ExtraMath.Lerp(_totalExternalForces.z, 0f, _linearDrag * Time.deltaTime);
            }
        }

        private void CheckExternalForces() {
            if (!HasExternalForces() && _physicsMode) {
                SetPhysicsMode(false);
                _totalExternalForces = Vector3.zero;
            }
        }

        // check if character is grounded (either via character controller or NavMeshAgent)
        private bool IsCharacterGrounded() {
            bool isGrounded;
            if (_physicsMode) {
                isGrounded = _characterController.isGrounded;
            } else {
                isGrounded = _navMeshAgent.isOnNavMesh;
            }
            return isGrounded;
        }

        private void ProcessGravity() {
            if (_totalExternalForces.y > Physics.gravity.y) {
                _totalExternalForces.y += Physics.gravity.y * Time.deltaTime;
            }
        }

        private bool HasExternalForces() {
            return _physicsMode &&
                (!Mathf.Approximately(_totalExternalForces.x, 0f) ||
                !Mathf.Approximately(_totalExternalForces.z, 0f) ||
                !IsGrounded);
        }

        private void SetMoveControllerValues() {
            if (_physicsMode || _currentMoveMode == CharacterMoveMode.Stopped) {
                MoveMagnitude = 0f;
                return;
            }
            Vector3 velocity = _navMeshAgent.velocity;
            Move = velocity.normalized;
            Rotation = Move; // TEMP
            MoveMagnitude = velocity.magnitude / _currentSpeed;
        }

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
            float magnitude = totalForce.magnitude;
            float reduction = Mathf.Max(1f - (_weight / magnitude), 0f);
            if(Mathf.Approximately(reduction, 0f)) {
                return;
            }
            Vector3 trueForce = totalForce.normalized * reduction;
            ClearDestination();
            SetPhysicsMode(true);
            if (overrideForce) {
                _totalExternalForces = Vector3.zero;
            }
            _totalExternalForces += totalForce;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            Vector3 totalForce = direction * force;
            AddForce(totalForce, overrideForce);
        }

        public void OverrideMovement(Vector3 direction) {
            Move = direction;
        }

        public void OverrideRotation(Vector3 direction) {
            Body.forward = direction;
        }

        public void SetDestination(Vector3 position) {
            if (_physicsMode) {
                return;
            }
            _pathing = _navMeshAgent.SetDestination(position);
            _navMeshAgent.updateRotation = true;
            if (!_pathing) {
                ClearDestination();
                OnArriveDestination?.Invoke(false);
            }
        }

        public void SetMoveMode(CharacterMoveMode moveMode) {
            // TODO: implement various speeds for characters
            _currentMoveMode = moveMode;
            switch (moveMode) {
                case CharacterMoveMode.Run:
                    _currentSpeed = _moveSpeed;
                    break;
                default:
                    _currentSpeed = 0f;
                    break;
            }
            _navMeshAgent.speed = _currentSpeed;
        }

        public void SetStoppingDistance(float distance) {
            _navMeshAgent.stoppingDistance = distance;
        }

        public void ClearDestination() {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.ResetPath();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if(_characterController.collisionFlags.HasFlag(CollisionFlags.Sides) && _physicsMode) {
                Vector3 normal = hit.normal;
                float dotProd = Vector3.Dot(_totalExternalForces.normalized, normal);
                Vector3 reduction = _totalExternalForces * dotProd;
                _totalExternalForces = new Vector3(0f, _totalExternalForces.y, 0f);
            }
        }
    }
}
