using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public enum CharacterActionContext {
        Invalid,
        Initiate,
        Hold,
        Release,
        Cancel
    }

    [System.Flags]
    public enum ActionPermissions {
        None = 0,
        Movement = 1 << 0,
        Rotation = 1 << 1,
        Animation = 1 << 2,
        All = ~0
    }

    public abstract class CharacterActionData : ScriptableObject {

        [SerializeField] protected ActionPermissions _permissions;

        public ActionPermissions Permissions => _permissions;

        public virtual CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, state);
            return response;
        }

        public virtual CharacterActionResponse Hold(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, state);
            return response;
        }

        public virtual CharacterActionResponse Release(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, state);
            return response;
        }
    }

    public interface ICharacterActionState {
        CharacterActionData Data { get; }
        AnimationData AnimationData { get; }
        bool Cancelable { get; }
        ActionStatus Status { get; set; }
        ActionPermissions Permissions { get; }

        void Clear();
    }

    public class CharacterActionResponse {
        public readonly bool Success;
        public readonly ICharacterActionState State;

        public CharacterActionResponse(bool success, ICharacterActionState state) {
            Success = success;
            State = state;
        }
    }
}
