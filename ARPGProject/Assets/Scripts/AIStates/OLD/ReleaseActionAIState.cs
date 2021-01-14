using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ReleaseActionAIState : AIState {

        [SerializeField] private NPCActionInfoProvider _npcActionInfoProvider;
        [SerializeField] private string _actionId;
        [SerializeField] private CharacterActionData _characterActionData;
        [SerializeField] private AIState _onActionCompleteState;

        private bool _released;

        public override void Enter() {
            base.Enter();
            _released = false;
            TryReleaseAction();
        }

        public override void Execute() {
            base.Execute();
            if (!_released) {
                TryReleaseAction();
            }
            // add condition for action release
        }

        private void TryReleaseAction() {
            _npcActionInfoProvider.AttemptAction(_actionId, CharacterActionContext.Release);
            if (_released) {
                _character.ActionController.CurrentState.OnActionStatusUpdated += OnActionStatusUpdated;
            }
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _character.ActionController.CurrentState.OnActionStatusUpdated -= OnActionStatusUpdated;
        }

        private void OnActionStatusUpdated(ICharacterActionState state, ActionStatus status) {
            if (status.HasFlag(ActionStatus.CanTransition) || status.HasFlag(ActionStatus.Completed)) {
                FireReadyToChangeState(_onActionCompleteState);
            }
        }
    }
}
