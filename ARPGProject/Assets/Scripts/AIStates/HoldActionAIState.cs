using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class HoldActionAIState : AIState {
        
        [SerializeField] private CharacterActionData _characterActionData;
        [SerializeField] private string _actionId;
        [SerializeField] private NPCActionInfoProvider _npcActionInfoProvider;
        [SerializeField] private AIState _onActionReadyState;

        private bool _initiated;

        public override void Enter() {
            base.Enter();
            _initiated = false;
            _npcActionInfoProvider.AttemptAction(_actionId, CharacterActionContext.Initiate);
        }

        public override void Execute() {
            base.Execute();
            if (!_initiated) {
                TryInitiateAction();
            } else {
                TryHoldAction();
            }
            // add condition for action release
        }

        private void TryInitiateAction() {
            _character.ActionController.OnCurrentActionUpdated += OnCurrentActionUpdated;
        }

        private void OnCurrentActionUpdated() {
            
            _character.ActionController.CurrentState.OnActionStatusUpdated += OnActionStatusUpdated;
        }

        private void TryHoldAction() {
            _npcActionInfoProvider.AttemptAction(_actionId, CharacterActionContext.Hold);
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _character.ActionController.CurrentState.OnActionStatusUpdated -= OnActionStatusUpdated;
        }

        private void OnActionStatusUpdated(ICharacterActionState state, ActionStatus status) {
            if (status.HasFlag(ActionStatus.Ready)) {
                FireReadyToChangeState(_onActionReadyState);
            }
        }
    }
}
