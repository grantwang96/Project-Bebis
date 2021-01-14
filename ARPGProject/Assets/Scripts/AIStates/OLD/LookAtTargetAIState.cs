using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class LookAtTargetAIState : AIState {

        [SerializeField] private NPCTargetManager _npcTargetManager;
        [SerializeField] private NPCMoveController3D _npcMoveController;
        [SerializeField] private AIStateMode _lookMode;

        public override void Enter() {
            base.Enter();
            if(_lookMode == AIStateMode.Enter) {
                SetRotation();
            }
        }

        public override void Execute() {
            base.Execute();
            if(_lookMode == AIStateMode.Execute) {
                SetRotation();
            }
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            if(_lookMode == AIStateMode.Exit) {
                SetRotation();
            }
        }

        private void SetRotation() {
            if(_npcTargetManager.CurrentTarget == null) {
                CustomLogger.Error(name, $"Current target is null!");
                return;
            }
            Vector3 targetPosition = _npcTargetManager.CurrentTarget.MoveController.Body.position;
            Vector3 currentPosition = _character.MoveController.Body.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0f;
            _npcMoveController.OverrideRotation(direction.normalized);
        }
    }
}
