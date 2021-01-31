using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class PlayAnimationAIState : AIStateV2
    {
        public const string Name = "PlayAnimation";
        private const string OnAnimationCompletedId = "OnAnimationCompleted";

        [SerializeField] private AIStateV2 _onAnimationCompleted;
        [SerializeField] private AnimationData _onEnterAnimationData;
        [SerializeField] private AnimationData _onExitAnimationData;
        // private readonly Dictionary<AIStateAnimationKey, AnimationData> _animationDatas = new Dictionary<AIStateAnimationKey, AnimationData>();

        public override void TryEnter() {
            // re-enter even if this is the current state
            // Debug.LogError("Try entering animation state " + Id);
            OnEnter();
        }

        public override void TryExit(IAIState nextState) {
            // exit even if the next state is this state
            if (_parentState != null) {
                _parentState.TryExit(nextState);
            }
            OnExit();
        }

        protected override void OnEnter() {
            base.OnEnter();
            _character.AnimationController.UpdateAnimationState(_onEnterAnimationData);
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
        }

        protected override void OnExit() {
            base.OnExit();
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.AnimationController.UpdateAnimationState(_onExitAnimationData);
        }

        private void OnAnimationStateUpdated(AnimationState animationState) {
            if(animationState == AnimationState.Completed) {
                FireReadyToTransition(_onAnimationCompleted);
            }
        }
    }
}
