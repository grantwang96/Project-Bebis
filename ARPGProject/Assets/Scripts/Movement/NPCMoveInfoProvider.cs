using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bebis {
    public class NPCMoveInfoProvider : MonoBehaviour, IMoveControllerInfoProvider {

        public Vector3 IntendedMoveDirection => GetIntendedMoveDirection();
        public Vector3 IntendedLookDirection => IntendedMoveDirection;
        public event Action<bool> OnArrivedDestination;

        [SerializeField] private NPCMoveController3D _npcMoveController;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        private bool _isPathing;

        private void OnMoveRestrictionUpdated() {
            if (_npcMoveController.MoveRestrictions.Restricted) {
                ClearDestination();
            }
        }

        public bool SetDestination(Vector3 targetDestination) {
            // do not attempt to path find if movement is restricted
            if (_npcMoveController.MoveRestrictions.Restricted) {
                return false;
            }
            bool success = _navMeshAgent.SetDestination(targetDestination);
            if (success) {
                _isPathing = true;
                _npcMoveController.MoveRestrictions.OnRestrictionUpdated += OnMoveRestrictionUpdated;
            }
            return success;
        }

        public void ClearDestination() {
            if (_navMeshAgent.enabled) {
                _navMeshAgent.ResetPath();
            }
        }

        private void FixedUpdate() {
            CheckArrivedDestination();
        }

        private void CheckArrivedDestination() {
            if (!_isPathing) {
                return;
            }
            if (Vector3.Distance(_navMeshAgent.nextPosition, _navMeshAgent.destination) <= _navMeshAgent.stoppingDistance) {
                _isPathing = false;
                OnArrivedDestination?.Invoke(true);
            }
        }

        private Vector3 GetIntendedMoveDirection() {
            return _navMeshAgent.desiredVelocity;
        }
    }
}
