using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class HoldActionAIState : AIState {
        
        [SerializeField] private CharacterActionData _characterActionData;
        [SerializeField] private AIState _onActionReadyState;

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
            } else {
                TryHoldAction();
            }
            // add condition for action release
        }

        private void TryInitiateAction() {
            _initiated = _character.ActionController.PerformAction(_characterActionData, CharacterActionContext.Initiate);
            if (_initiated) {
                _character.ActionController.OnActionStatusUpdated += OnActionStatusUpdated;
            }
        }

        private void TryHoldAction() {
            _character.ActionController.PerformAction(_characterActionData, CharacterActionContext.Hold);
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _character.ActionController.OnActionStatusUpdated -= OnActionStatusUpdated;
        }

        private void OnActionStatusUpdated(ActionStatus status) {
            if (status.HasFlag(ActionStatus.Ready)) {
                FireReadyToChangeState(_onActionReadyState);
            }
        }
    }
}
