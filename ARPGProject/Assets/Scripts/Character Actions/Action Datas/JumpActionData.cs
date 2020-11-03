using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Jump")]
    public class JumpActionData : CharacterActionData {

        [SerializeField] private float _jumpForce;
        [SerializeField] private Vector3 _jumpDirection;
        [SerializeField] private AnimationData _animationData;

        public float JumpForce => _jumpForce;
        public Vector3 JumpDirection => _jumpDirection;
        public AnimationData AnimationData => _animationData;

        private bool CanJump(ICharacter character, ICharacterActionState state) {
            return character.MoveController.IsGrounded &&
                (state == null || state.Data.Priority < Priority);
        }

        protected override bool CanPerformAction(ICharacter character, ICharacterActionState foundActionState) {
            return CanJump(character, character.ActionController.CurrentState);
        }

        protected override ICharacterActionState CreateActionState(ICharacter character) {
            return new JumpActionState(this, character);
        }
    }

    public class JumpActionState : CharacterActionState {

        private JumpActionData _data;

        public JumpActionState(JumpActionData data, ICharacter character) : base(data, character) {
            _data = data;
        }

        public override void Initiate() {
            base.Initiate();
            _character.MoveController.AddForce(CalculateDirection(_data, _character), true);
            _character.MoveController.MoveRestrictions.AddRestriction(nameof(JumpActionState));
            _character.MoveController.LookRestrictions.AddRestriction(nameof(JumpActionState));
            _character.AnimationController.UpdateAnimationState(_data.AnimationData);
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentActionUpdated += OnCurrentActionUpdated;
        }

        public override void Clear() {
            base.Clear();
            _character.MoveController.MoveRestrictions.RemoveRestriction(nameof(JumpActionState));
            _character.MoveController.LookRestrictions.RemoveRestriction(nameof(JumpActionState));
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentActionUpdated -= OnCurrentActionUpdated;
        }

        private void OnCurrentActionUpdated() {
            if(_character.ActionController.CurrentState != this) {
                UpdateActionStatus(ActionStatus.Completed);
            }
        }

        private void OnAnimationStateUpdated(AnimationState animationState) {
            switch (animationState) {
                case AnimationState.Started:
                    UpdateActionStatus(ActionStatus.Started);
                    break;
                case AnimationState.InProgress:
                    UpdateActionStatus(ActionStatus.InProgress);
                    break;
                case AnimationState.CanTransition:
                    UpdateActionStatus(ActionStatus.CanTransition);
                    break;
                case AnimationState.Completed:
                    UpdateActionStatus(ActionStatus.Completed);
                    break;
            }
        }

        private static Vector3 CalculateDirection(JumpActionData data, ICharacter character) {
            Vector3 direction = data.JumpDirection + character.MoveController.Move;
            Vector3 jumpForce = direction.normalized * data.JumpForce;
            return jumpForce;
        }
    }
}
