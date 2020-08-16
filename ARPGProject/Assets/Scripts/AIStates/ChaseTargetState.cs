using UnityEngine;

namespace Bebis {
    public class ChaseTargetState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private NPCMoveController3D _npcMoveController;
        [SerializeField] private AIState _onTargetLostState;
        [SerializeField] private AIState _onArrivedTargetState;
        [SerializeField] private float _arrivalRadius;
        [SerializeField] private CharacterMoveMode _moveMode;

        public override void Enter() {
            base.Enter();
            _npcMoveController.SetStoppingDistance(_arrivalRadius);
            _npcMoveController.OnArriveDestination += OnArriveDestination;
            _npcMoveController.SetMoveMode(_moveMode);
        }

        public override void Execute() {
            base.Execute();
            _npcMoveController.SetDestination(_npcNavigator.TargetPosition);
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _npcMoveController.OnArriveDestination -= OnArriveDestination;
            _npcMoveController.SetMoveMode(CharacterMoveMode.Stopped);
        }

        private void OnArriveDestination(bool success) {
            AIState nextState = success ? _onArrivedTargetState : _onTargetLostState;
            FireReadyToChangeState(nextState);
        }
    }
}
