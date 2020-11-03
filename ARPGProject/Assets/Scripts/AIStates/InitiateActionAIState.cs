using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class InitiateActionAIState : AIState {

        [SerializeField] private NPCActionInfoProvider _npcActionInfoProvider;
        [SerializeField] private string _actionId;
        [SerializeField] private CharacterActionData _characterActionData;
        [SerializeField] private AIState _onActionCompletedState;

        private bool _initiated;

        public override void Enter() {
            base.Enter();
            _initiated = false;
            TryInitiateAction();
        }

        public override void Execute() {
            base.Execute();
            if (!_initiated) {
                TryInitiateAction();
            }
        }

        private void TryInitiateAction() {
            _npcActionInfoProvider.AttemptAction(_actionId, CharacterActionContext.Initiate);
            if (_initiated) {
                _character.ActionController.CurrentState.OnActionStatusUpdated += OnActionCompleted;
            }
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _initiated = false;
            _character.ActionController.CurrentState.OnActionStatusUpdated -= OnActionCompleted;
        }

        private void OnActionCompleted(ICharacterActionState state, ActionStatus status) {
            if(status.HasFlag(ActionStatus.CanTransition) || status.HasFlag(ActionStatus.Completed)) {
                FireReadyToChangeState(_onActionCompletedState);
            }
        }
    }
}
