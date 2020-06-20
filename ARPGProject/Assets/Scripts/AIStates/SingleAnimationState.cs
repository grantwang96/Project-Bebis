using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class SingleAnimationState : AIState {
        [SerializeField] private AIState _onAnimationCompleted;

        public override void Enter() {
            base.Enter();
            _character.AnimationController.UpdateAnimationState(_animationData);
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
        }

        private void OnAnimationStateUpdated(AnimationState state) {
            if(state == AnimationState.Completed) {
                FireReadyToChangeState(_onAnimationCompleted);
            }
        }
    }
}
