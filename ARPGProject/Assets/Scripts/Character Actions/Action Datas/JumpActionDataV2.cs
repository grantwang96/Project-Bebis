using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Jump")]
    public class JumpActionDataV2 : CharacterActionDataV2
    {
        [SerializeField] private float _jumpForce;
        [SerializeField] private Vector3 _jumpDirection;
        [SerializeField] private AnimationData _animationData; 

        public float JumpForce => _jumpForce;
        public Vector3 JumpDirection => _jumpDirection;
        public AnimationData AnimationData => _animationData;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new JumpActionStateV2(this, character);
        }
    }

    public class JumpActionStateV2 : CharacterActionStateV2
    {
        private JumpActionDataV2 _data;

        public JumpActionStateV2(JumpActionDataV2 data, ICharacterV2 character) : base(data, character) {
            _data = data;
        }

        public override bool CanPerform(CharacterActionContext context) {
            bool canPerform = base.CanPerform(context);
            return canPerform && _character.MoveController.IsGrounded;
        }

        protected override void OnPerformAction(CharacterActionContext context) {
            base.OnPerformAction(context);
            // _character.MoveController.MoveRestrictions.AddRestriction(nameof(JumpActionState));
            _character.MoveController.LookRestrictions.AddRestriction(_data.Id);
            _character.AnimationController.UpdateAnimationState(_data.AnimationData);
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated += OnCurrentStateUpdated;
            _character.MoveController.OnIsGroundedUpdated += OnCharacterHitGround;
            _character.MoveController.AddForce(CalculateDirection(_data, _character), true);
        }

        public override void Clear() {
            base.Clear();
            // _character.MoveController.MoveRestrictions.RemoveRestriction(nameof(JumpActionState));
            _character.MoveController.LookRestrictions.RemoveRestriction(_data.Id);
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated -= OnCurrentStateUpdated;
            _character.MoveController.OnIsGroundedUpdated -= OnCharacterHitGround;
            Status = ActionStatus.Ready;
        }

        private void OnCurrentStateUpdated(ICharacterActionStateV2 newState) {
            if (_character.ActionController.CurrentState != this) {
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

        private void OnCharacterHitGround(bool isGrounded) {
            if (isGrounded) {
                UpdateActionStatus(ActionStatus.Completed);
            }
        }

        private static Vector3 CalculateDirection(JumpActionDataV2 data, ICharacterV2 character) {
            Vector3 direction = data.JumpDirection + character.UnitController.MovementInput;
            Vector3 jumpForce = direction.normalized * data.JumpForce;
            return jumpForce;
        }
    }
}