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
            if (!character.MoveController.CanJump) {
                return base.Initiate(character, state, context);
            }
            JumpActionState newState = new JumpActionState(this, character);
            CharacterActionResponse response = new CharacterActionResponse(true, newState);
            return response;
        }
    }

    public class JumpActionState : CharacterActionState {

        public JumpActionState(JumpActionData data, ICharacter character) : base(data, character) {
            // TODO: may wanna animation drive this behaviour. temporary immediate complete for now
            Status = ActionStatus.Completed;

            _character.MoveController.AddForce(data.JumpDirection, data.JumpForce, true);
        }
    }
}
