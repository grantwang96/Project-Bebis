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
            Vector3 nextPosition = _npcNavigator.GetRandomLocationAtDistanceRadius(_wanderRange.GetRandomFromMinMax());
            _npcNavigator.TargetPosition = nextPosition;
            FireReadyToChangeState(_onIdleFinishState);
        }
    }
}
