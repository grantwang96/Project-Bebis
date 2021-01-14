using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class ListenForHitAIState : AIStateV2
    {
        public const string Name = "ListenForHit";
        private const string OnHitStateTransitionId = "OnHit"; // the transition id to look for when setting "On Hit"

        private IAIState _onHitState;

        public ListenForHitAIState(ICharacterV2 character, AIStateMachineV2 stateMachine, AIStateData stateData, IAIState parentState = null) :
            base(character, stateMachine, stateData, parentState) {

        }

        public override void Initialize() {
            base.Initialize();
            TrySetOnHitState();
        }

        protected override void OnEnter() {
            base.OnEnter();
            // start listening for damage events
            _character.Damageable.OnReceivedHit += OnReceivedHit;
        }

        protected override void OnExit() {
            base.OnExit();
            // stop listening for damage events
            _character.Damageable.OnReceivedHit -= OnReceivedHit;
        }

        private void TrySetOnHitState() {
            // attempt to find the corresponding transition id
            int index = _stateData.TransitionDatas.FindIndex(x => x.Key.Equals(OnHitStateTransitionId));
            if(index == -1) {
                Debug.LogError($"[{Id}]: Could not retrieve \'{OnHitStateTransitionId}\' transition id from state data {_stateName}");
                return;
            }
            // attempt to retrieve the corresponding state for "On Hit"
            if (!_stateMachine.TryGetAIState(_stateData.TransitionDatas[index].Value, out _onHitState)) {
                Debug.LogError($"[{Id}]: Could not retreive state with Id {_stateData.TransitionDatas[index].Value} from state machine!");
            }
        }

        private void OnReceivedHit(HitEventInfoV2 hitEventInfo) {
            FireReadyToTransition(_onHitState);
        }
    }
}
