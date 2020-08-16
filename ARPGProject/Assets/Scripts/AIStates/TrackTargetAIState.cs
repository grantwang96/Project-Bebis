using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TrackTargetAIState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private NPCTargetManager _npcTargetManager;

        [SerializeField] private AIState _onTargetLostState;

        public override void Enter() {
            base.Enter();
            _npcTargetManager.OnCurrentTargetSet += OnCurrentTargetUpdated;
            OnCurrentTargetUpdated();
        }

        public override void Execute() {
            base.Execute();
            if(_npcTargetManager.CurrentTarget == null) {
                FireReadyToChangeState(_onTargetLostState);
                return;
            }
            _npcNavigator.TargetPosition = _npcTargetManager.CurrentTarget.MoveController.Body.position;
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _npcTargetManager.OnCurrentTargetSet -= OnCurrentTargetUpdated;
        }

        private void OnCurrentTargetUpdated() {
            if(_npcTargetManager.CurrentTarget == null) {
                FireReadyToChangeState(_onTargetLostState);
            }
        }
    }
}
