using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Defend")]
    public class DefendActionData : CharacterActionData {

        [SerializeField] private AnimationData _onReleaseAnimationData;
        public AnimationData OnReleaseAnimationData => _onReleaseAnimationData;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if (state != null && state.Data != this) {
                return base.Initiate(character, state, context);
            }
            DefendActionState defendActionState = new DefendActionState(this, character);
            CharacterActionResponse response = new CharacterActionResponse(true, true, defendActionState);
            return response;
        }

        public override CharacterActionResponse Hold(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if (state != null && state.Data != this) {
                return base.Hold(character, state, context);
            }
            ICharacterActionState defendState = state;
            if(state == null) {
                defendState = new DefendActionState(this, character);
            }
            CharacterActionResponse response = new CharacterActionResponse(true, true, defendState);
            return response;
        }

        public override CharacterActionResponse Release(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(true, true, state);
            if(state != null && state.Data == this) {
                state.Status = ActionStatus.Completed;
            }
            return response;
        }
    }

    public class DefendActionState : CharacterActionState {

        private DefendActionData _data;

        public DefendActionState(DefendActionData data, ICharacter character) : base(data, character) {
            _data = data;
        }

        public override void Clear() {
            base.Clear();
            _character.AnimationController.UpdateAnimationState(_data.OnReleaseAnimationData);
        }
    }
}
