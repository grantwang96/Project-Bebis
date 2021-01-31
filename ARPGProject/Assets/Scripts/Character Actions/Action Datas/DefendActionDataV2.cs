using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Defend")]
    public class DefendActionDataV2 : CharacterActionDataV2
    {

        [SerializeField] private AnimationData _startDefendingAnimationData;
        [SerializeField] private AnimationData _onReleaseAnimationData;
        public AnimationData StartDefendingAnimationData => _startDefendingAnimationData;
        public AnimationData OnReleaseAnimationData => _onReleaseAnimationData;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new DefendActionStateV2(this, character);
        }
    }

    public class DefendActionStateV2 : CharacterActionStateV2
    {
        private DefendActionDataV2 _data;

        public DefendActionStateV2(DefendActionDataV2 data, ICharacterV2 character) : base(data, character) {
            _data = data;
            character.ActionController.OnCurrentStateUpdated += OnCurrentStateUpdated;
        }

        public override bool CanInterrupt(CharacterActionContext context, ICharacterActionDataV2 data) {
            return (Data.AllowedTransitionMoves & data.Tags) != 0;
        }

        public override bool CanPerform(CharacterActionContext context) {
            return true;
        }

        public override void Clear() {
            base.Clear();
            StopDefend();
            _character.ActionController.OnCurrentStateUpdated -= OnCurrentStateUpdated;
        }

        private void PerformDefend() {
            _character.AnimationController.UpdateAnimationState(_data.StartDefendingAnimationData);
            if (!_character.MoveController.MovementRestrictions.ContainsId(_data.Id)) {
                _character.MoveController.OverrideMovement(Vector3.zero);
                _character.MoveController.MovementRestrictions.AddRestriction(_data.Id);
            }
        }

        private void StopDefend() {
            _character.AnimationController.UpdateAnimationState(_data.OnReleaseAnimationData);
            _character.MoveController.MovementRestrictions.RemoveRestriction(_data.Id);
        }

        private void OnCurrentStateUpdated(ICharacterActionStateV2 state) {
            // if a new state has been set, this character is no longer defending
            if(state != this) {
                StopDefend();
            }
        }
    }
}
