using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class MoveState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;

        [SerializeField] private AIState _onArrivedSuccessState;
        [SerializeField] private AIState _onFailedToPathState;

        private List<IntVector3> _path = new List<IntVector3>();
        [SerializeField] private IntVector3 _currentDestination;
        [SerializeField] private IntVector3 _currentPosition;

        public override void Enter() {
            _path.Clear();
            Vector2 position = _character.MoveController.Body.position;
            _currentPosition = LevelDataManager.GetMapPosition(position);
            PathStatus status = MapService.GetPathToDestination(_currentPosition, _npcNavigator.MoveTarget, _path);
            if(status == PathStatus.Invalid) {
                FireReadyToChangeState(_onFailedToPathState);
                return;
            }
            SetNextMoveTarget();
            base.Enter();
        }

        public override void Execute() {
            base.Execute();
            _currentPosition = LevelDataManager.GetMapPosition(_character.MoveController.Body.position);
            if(_currentPosition == _currentDestination) {
                SetNextMoveTarget();
            }
        }

        private void SetNextMoveTarget() {
            if(_path.Count == 0) {
                _npcNavigator.MoveInput = new Vector2(0, 0);
                FireReadyToChangeState(_onArrivedSuccessState);
                return;
            }
            _currentDestination = _path[0];
            _path.RemoveAt(0);
            IntVector3 direction = _currentDestination - _currentPosition;
            _npcNavigator.MoveInput = new Vector2(direction.x, direction.y);
        }
    }

    public class MoveStateInitializationData : AIStateInitializationData {
        public int X;
        public int Y;
    }
}
