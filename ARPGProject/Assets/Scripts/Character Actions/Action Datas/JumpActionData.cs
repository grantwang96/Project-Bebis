using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Jump")]
    public class JumpActionData : CharacterActionData {

        [SerializeField] private float _jumpForce;
        [SerializeField] private Vector3 _jumpDirection;

        public float JumpForce => _jumpForce;
        public Vector3 JumpDirection => _jumpDirection;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if (!CanJump(character, state)) {
                bool bufferable = state.Data != this;
                return new CharacterActionResponse(false, bufferable, state);
            }
            JumpActionState newState = new JumpActionState(this, character);
            CharacterActionResponse response = new CharacterActionResponse(true, true, newState);
            return response;
        }

        private bool CanJump(ICharacter character, ICharacterActionState state) {
            return character.MoveController.IsGrounded &&
                (state == null || state.Data.Priority < Priority ||
                state.Status.HasFlag(ActionStatus.CanTransition) ||
                state.Status.HasFlag(ActionStatus.Completed));
        }
    }

    public class JumpActionState : CharacterActionState {

        public JumpActionState(JumpActionData data, ICharacter character) : base(data, character) {
            // _character.MoveController.AddForce(data.JumpDirection, data.JumpForce, true);
            _character.MoveController.AddForce(CalculateDirection(data, character), true);
        }

        private static Vector3 CalculateDirection(JumpActionData data, ICharacter character) {
            Vector3 direction = data.JumpDirection + character.MoveController.Move;
            Vector3 jumpForce = direction.normalized * data.JumpForce;
            return jumpForce;
        }
    }
}
