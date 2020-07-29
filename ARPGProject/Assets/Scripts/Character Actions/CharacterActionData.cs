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

        [SerializeField] protected bool _cancelable;
        [SerializeField] protected AnimationData _animationData;
        [SerializeField] protected ActionPermissions _permissions;
        [SerializeField] protected int _priority;
        [SerializeField] protected bool _bufferable;

        public bool Cancelable => _cancelable;
        public AnimationData AnimationData => _animationData;
        public ActionPermissions Permissions => _permissions;
        public int Priority => _priority;
        public bool Bufferable => _bufferable;

        public virtual CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, false, state);
            return response;
        }

        public virtual CharacterActionResponse Hold(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, false, state);
            return response;
        }

        public virtual CharacterActionResponse Release(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            CharacterActionResponse response = new CharacterActionResponse(false, false, state);
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

    public class CharacterActionState : ICharacterActionState {

        public CharacterActionData Data { get; }
        public AnimationData AnimationData { get; }
        public bool Cancelable { get; }
        public ActionStatus Status { get; set; }
        public ActionPermissions Permissions { get; }

        protected ICharacter _character;

        public CharacterActionState(CharacterActionData data, ICharacter character) {
            Data = data;
            AnimationData = data.AnimationData;
            Cancelable = data.Cancelable;
            Permissions = data.Permissions;

            _character = character;
            _character.ActionController.OnActionStatusUpdated += OnActionStatusUpdated;
        }

        public virtual void Clear() {
            _character.ActionController.OnActionStatusUpdated -= OnActionStatusUpdated;
        }

        protected virtual void OnActionStatusUpdated(ActionStatus status) {
            if (status == ActionStatus.Completed) {
                Clear();
            }
        }
    }

    public class CharacterActionResponse {
        public readonly bool Success;
        public readonly bool Bufferable;
        public readonly ICharacterActionState State;

        public CharacterActionResponse(bool success, bool bufferable, ICharacterActionState state) {
            Success = success;
            Bufferable = bufferable;
            State = state;
        }
    }
}
