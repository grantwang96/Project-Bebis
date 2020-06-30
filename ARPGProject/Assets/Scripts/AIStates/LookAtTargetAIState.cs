using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class LookAtTargetAIState : AIState {

        [SerializeField] private NPCTargetManager _npcTargetManager;
        [SerializeField] private NPCNavigator _npcNavigator;

        public override void Execute() {
            base.Execute();
            SetRotation();
        }

        private void SetRotation() {
            Vector3 targetPosition = _npcTargetManager.CurrentTarget.MoveController.Body.position;
            Vector3 currentPosition = _character.MoveController.Body.position;
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0f;
            _npcNavigator.RotationInput = direction;
        }
    }
}
