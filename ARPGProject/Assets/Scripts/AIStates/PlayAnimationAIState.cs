using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class PlayAnimationAIState : AIStateV2
    {
        public const string Name = "PlayAnimation";
        private const string OnAnimationCompletedId = "OnAnimationCompleted";

        private IAIState _onAnimationCompleted;
        private readonly Dictionary<AIStateAnimationKey, AnimationData> _animationDatas = new Dictionary<AIStateAnimationKey, AnimationData>();

        public PlayAnimationAIState(ICharacterV2 character, AIStateMachineV2 stateMachine, AIStateData stateData, IAIState parentState = null) :
            base(character, stateMachine, stateData, parentState) {

        }

        public override void Initialize() {
            base.Initialize();
            TryGetOnAnimationCompletedState();
            BuildAnimationDatas();
        }

        protected override void OnEnter() {
            base.OnEnter();
            if (_animationDatas.ContainsKey(AIStateAnimationKey.OnEnter)) {
                _character.AnimationController.UpdateAnimationState(_animationDatas[AIStateAnimationKey.OnEnter]);
                _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            }
        }

        protected override void OnExit() {
            base.OnExit();
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            if (_animationDatas.ContainsKey(AIStateAnimationKey.OnExit)) {
                _character.AnimationController.UpdateAnimationState(_animationDatas[AIStateAnimationKey.OnExit]);
            }
        }

        private void TryGetOnAnimationCompletedState() {
            // attempt to find the corresponding transition id
            int index = _stateData.TransitionDatas.FindIndex(x => x.Key.Equals(OnAnimationCompletedId));
            if (index == -1) {
                Debug.LogError($"[{Id}]: Could not retrieve \'{OnAnimationCompletedId}\' transition id from state data {_stateName}");
                return;
            }
            // attempt to retrieve the corresponding state for "On Animation Completed"
            if (!_stateMachine.TryGetAIState(_stateData.TransitionDatas[index].Value, out _onAnimationCompleted)) {
                Debug.LogError($"[{Id}]: Could not retreive state with Id {_stateData.TransitionDatas[index].Value} from state machine!");
            }
        }

        private void BuildAnimationDatas() {
            for(int i = 0; i < _stateData.AnimationDatas.Count; i++) {
                if (_animationDatas.ContainsKey(_stateData.AnimationDatas[i].Key)) {
                    continue;
                }
                _animationDatas.Add(_stateData.AnimationDatas[i].Key, _stateData.AnimationDatas[i].Value);
            }
        }

        private void OnAnimationStateUpdated(AnimationState animationState) {
            if(animationState == AnimationState.Completed) {
                FireReadyToTransition(_onAnimationCompleted);
            }
        }
    }
}
