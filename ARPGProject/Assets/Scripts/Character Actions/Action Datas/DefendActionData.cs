using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Defend")]
    public class DefendActionData : CharacterActionData {

        [SerializeField] private AnimationData _startDefendingAnimationData;
        [SerializeField] private AnimationData _onReleaseAnimationData;
        public AnimationData StartDefendingAnimationData => _startDefendingAnimationData;
        public AnimationData OnReleaseAnimationData => _onReleaseAnimationData;

        protected override bool CanPerformAction(ICharacter character, ICharacterActionState foundActionState) {
            ICharacterActionState currentState = character.ActionController.CurrentState;
            return currentState == null || currentState.Data == this;
        }

        protected override ICharacterActionState CreateActionState(ICharacter character) {
            return new DefendActionState(this, character);
        }
    }

    public class DefendActionState : CharacterActionState {

        private DefendActionData _data;

        public DefendActionState(DefendActionData data, ICharacter character) : base(data, character) {
            _data = data;
        }

        public override void Initiate() {
            base.Initiate();
            _character.AnimationController.UpdateAnimationState(_data.StartDefendingAnimationData);
            _character.MoveController.OverrideMovement(Vector3.zero);
            _character.MoveController.MoveRestrictions.AddRestriction(nameof(DefendActionState));
        }

        public override void Release() {
            base.Release();
            UpdateActionStatus(ActionStatus.Completed);
            Debug.Log("Release");
        }

        public override void Clear() {
            base.Clear();
            _character.AnimationController.UpdateAnimationState(_data.OnReleaseAnimationData);
            _character.MoveController.MoveRestrictions.RemoveRestriction(nameof(DefendActionState));
        }
    }
}
