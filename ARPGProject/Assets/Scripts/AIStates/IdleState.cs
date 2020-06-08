using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class IdleState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private MinMax_Float _idleTimeRange;

        [SerializeField] private MinMax_Int _wanderRange;
        [SerializeField] private AIState _onIdleFinishState;

        [SerializeField] private float _idleTime;

        public override void Enter() {
            base.Enter();
            _idleTime = Time.time + Random.Range(_idleTimeRange.Min, _idleTimeRange.Max);
        }

        public override void Execute() {
            base.Execute();
            if(Time.time >= _idleTime) {
                IdleFinished();
                return;
            }
        }

        private void IdleFinished() {
            Vector2 position = _character.MoveController.Body.transform.position;
            IntVector3 intPosition = new IntVector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
            List<IntVector3> possiblePositions = MapService.GetPositionsWithinRadius(_wanderRange.Min, intPosition, _wanderRange.Max);
            if(possiblePositions.Count == 0) {
                CustomLogger.Warn(name, $"No available spaces to wander to!");
                FireReadyToChangeState(this);
            }
            IntVector3 newWanderPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];
            _npcNavigator.MoveTarget = newWanderPosition;
            FireReadyToChangeState(_onIdleFinishState);
        }
    }
}
