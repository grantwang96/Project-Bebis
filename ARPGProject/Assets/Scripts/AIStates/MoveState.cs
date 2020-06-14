using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bebis {
    public class MoveState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private AIState _onArrivedSuccessState;
        [SerializeField] private AIState _onFailedToPathState;
        [SerializeField] private float _targetRadius;

        private bool _pathSet = false;
        [SerializeField] private Vector3[] _pathCorners;
        [SerializeField] private int _pathIndex;
        private Vector3 _characterPosition => _character.MoveController.Body.position;

        public override void Enter() {
            base.Enter();
            _pathSet = false;
            GeneratePath();
        }

        public override void Execute() {
            base.Execute();
            if (!_pathSet) {
                return;
            }
            if (HasReachedCurrentNode()) {
                OnPathNodeReached();
            }
            SetMoveDirection();
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _npcNavigator.MoveInput = Vector3.zero;
        }

        private void GeneratePath() {
            _npcNavigator.OnCalculatePathCompleted += OnPathCalculated;
            _npcNavigator.CalculatePath(_npcNavigator.TargetPosition);
        }

        private void OnPathCalculated(NavMeshPath path) {
            _npcNavigator.OnCalculatePathCompleted -= OnPathCalculated;
            if(path.status == NavMeshPathStatus.PathInvalid) {
                FireReadyToChangeState(_onFailedToPathState);
            }
            _pathSet = true;
            _pathCorners = path.corners;
            _pathIndex = 0;
        }

        private bool HasReachedCurrentNode() {
            float distance = Vector3.Distance(_characterPosition, _pathCorners[_pathIndex]);
            return distance <= _targetRadius;
        }

        private void OnPathNodeReached() {
            _pathIndex++;
            if(_pathIndex >= _pathCorners.Length) {
                FireReadyToChangeState(_onArrivedSuccessState);
                return;
            }
        }

        private void SetMoveDirection() {
            if(_pathIndex >= _pathCorners.Length) {
                return;
            }
            _npcNavigator.MoveInput = (_pathCorners[_pathIndex] - _characterPosition).normalized;
        }
    }
}
