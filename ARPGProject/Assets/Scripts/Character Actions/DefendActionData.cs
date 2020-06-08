using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Defend")]
    public class DefendActionData : CharacterActionData {
        public override CharacterActionResponse Hold(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if(state != null && state.Data != this) {
                return new CharacterActionResponse(false, state);
            }
            return base.Hold(character, state, context);
        }
    }

    public class DefendActionState : CharacterActionState {

        public DefendActionState(DefendActionData data, ICharacter character) : base(data, character) {
            
        }

        protected override void OnActionStatusUpdated(ActionStatus status) {
            base.OnActionStatusUpdated(status);
        }
    }
}
