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
        public bool IsGrounded => true;
        public float Height => _characterController.height;

        public NavMeshPathStatus PathStatus => _navMeshAgent.path?.status ?? NavMeshPathStatus.PathInvalid;
        
        [SerializeField] private bool _canMove = true;
        [SerializeField] private bool _canRotate = true;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _linearDrag = 1f;
        [SerializeField] private Transform _bodyRoot;
        [SerializeField] private Transform _bodyCenter;
        [SerializeField] private CharacterController _characterController;

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        private bool _overrideMovement;
        private bool _overrideRotation;
        private bool _physicsMode;
        [SerializeField] private Vector3 _totalExternalForces;

        public event Action<bool> OnArriveDestination;

        private void FixedUpdate() {
            CheckArrivedDestination();
            ProcessExternalForces();
        }

        private void CheckArrivedDestination() {
            if (!_navMeshAgent.hasPath || _physicsMode) {
                return;
            }
            float distance = Vector3.Distance(_navMeshAgent.nextPosition, _npcNavigator.TargetPosition);
            if(distance <= _navMeshAgent.stoppingDistance) {
                ClearDestination();
                OnArriveDestination?.Invoke(true);
            }
        }

        private void SetPhysicsMode(bool mode) {
            _physicsMode = mode;
            _characterController.enabled = mode;
            _navMeshAgent.updatePosition = !mode;
            _navMeshAgent.updateRotation = !mode;
            _navMeshAgent.nextPosition = transform.position;
        }

        private void ProcessExternalForces() {
            if (!_physicsMode) {
                return;
            }
            ProcessDrag();
            ProcessGravity();
            CheckExternalForces();
            _characterController.Move(_totalExternalForces * Time.deltaTime);
        }

        private void ProcessDrag() {
            if (HasExternalForces()) {
                return;
            }
            if (_characterController.isGrounded) {
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

        public void AddForce(Vector3 totalForce, bool overrideForce = false) {
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
            // _move = direction;
            _overrideMovement = true;
        }

        public void OverrideRotation(Vector3 direction) {
            // _rotation = direction;
            _overrideRotation = true;
        }

        public void SetDestination(Vector3 position) {
            if (_physicsMode) {
                return;
            }
            bool success = _navMeshAgent.SetDestination(position);
            if (!success) {
                ClearDestination();
                OnArriveDestination?.Invoke(false);
            }
        }

        public void ClearDestination() {
            _navMeshAgent.ResetPath();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if(_characterController.collisionFlags.HasFlag(CollisionFlags.Sides) && _physicsMode) {
                Vector3 normal = hit.normal;
                float dotProd = Vector3.Dot(_totalExternalForces.normalized, normal);
                Debug.Log(dotProd);
                Vector3 reduction = _totalExternalForces * dotProd;
                _totalExternalForces = new Vector3(0f, _totalExternalForces.y, 0f);
            }
        }
    }
}
