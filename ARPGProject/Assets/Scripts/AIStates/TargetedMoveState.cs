using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bebis {
    public class TargetedMoveState : AIState {

        [SerializeField] private NPCNavigator _npcNavigator;
        [SerializeField] private NPCMoveController3D _npcMoveController;
        [SerializeField] private AIState _onArrivedSuccessState;
        [SerializeField] private AIState _onFailedToPathState;
        [SerializeField] private bool _setRotation;
        
        private Vector3 _characterPosition => _character.MoveController.Body.position;

        public override void Enter() {
            base.Enter();
            SetDestination();
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _npcNavigator.MoveInput = Vector3.zero;
        }

        private void SetDestination() {
            _npcMoveController.OnArriveDestination += OnArrivedDestination;
            _npcMoveController.SetDestination(_npcNavigator.TargetPosition);
        }

        private void OnArrivedDestination(bool success) {
            _npcMoveController.OnArriveDestination -= OnArrivedDestination;
            _npcMoveController.ClearDestination();
            if (success) {
                FireReadyToChangeState(_onArrivedSuccessState);
            } else {
                FireReadyToChangeState(_onFailedToPathState);
            }
        }
    }
}
