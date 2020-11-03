using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Resources;
using TMPro;

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
        Walk = 1 << 3,
        Run = 1 << 4,
        All = ~0
    }

    public interface ICharacterActionData
    {

        string Id { get; }
        int Priority { get; }

        CharacterActionResponse Initiate(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context);
        CharacterActionResponse Hold(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context);
        CharacterActionResponse Release(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context);
    }

    public abstract class CharacterActionData : ScriptableObject, ICharacterActionData {

        [SerializeField] private string _id;
        [SerializeField] private string _actionName;

        [SerializeField] protected bool _cancelable;
        [SerializeField] protected int _priority;
        [SerializeField] protected bool _bufferable;

        public string Id => _id;
        public string ActionName => _actionName;
        public bool Cancelable => _cancelable;
        public int Priority => _priority;
        public bool Bufferable => _bufferable;

        public virtual CharacterActionResponse Initiate(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            ICharacterActionState currentAction = character.ActionController.CurrentState;
            CharacterActionResponse response = new CharacterActionResponse();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState)) {
                response.Success = false;
                response.State = currentAction;
                return response;
            }
            // if there is no corresponding state in the action controller, create one
            if (foundActionState == null) {
                foundActionState = CreateActionState(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = foundActionState;
            foundActionState.Initiate();
            return response;
        }

        public virtual CharacterActionResponse Hold(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            ICharacterActionState currentAction = character.ActionController.CurrentState;
            CharacterActionResponse response = new CharacterActionResponse();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState)) {
                response.Success = false;
                response.State = currentAction;
                return response;
            }
            // if there is no corresponding state in the action controller, create one
            if (foundActionState == null) {
                foundActionState = CreateActionState(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = foundActionState;
            foundActionState.Hold();
            return response;
        }

        public virtual CharacterActionResponse Release(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            ICharacterActionState currentAction = character.ActionController.CurrentState;
            CharacterActionResponse response = new CharacterActionResponse();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState)) {
                response.Success = false;
                response.State = currentAction;
                return response;
            }
            // if there is no corresponding state in the action controller, create one
            if (foundActionState == null) {
                foundActionState = CreateActionState(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = foundActionState;
            foundActionState.Release();
            return response;
        }

        protected virtual bool CanPerformAction(ICharacter character, ICharacterActionState foundActionState) {
            return true;
        }

        protected CharacterActionResponse FailedActionResponse(ICharacter character) {
            return new CharacterActionResponse(false, false, character.ActionController.CurrentState);
        }

        protected abstract ICharacterActionState CreateActionState(ICharacter character);
    }

    public interface ICharacterActionState {
        CharacterActionData Data { get; }
        bool Cancelable { get; }
        ActionStatus Status { get; set; }
        void Initiate();
        void Hold();
        void Release();

        event Action<ICharacterActionState, ActionStatus> OnActionStatusUpdated;

        void Clear();
    }

    public class CharacterActionState : ICharacterActionState {

        public CharacterActionData Data { get; }
        public bool Cancelable { get; }
        public ActionStatus Status { get; set; }

        public event Action<ICharacterActionState, ActionStatus> OnActionStatusUpdated;

        protected ICharacter _character;

        public CharacterActionState(CharacterActionData data, ICharacter character) {
            Data = data;
            Cancelable = data.Cancelable;

            _character = character;
        }

        public virtual void Initiate() { }
        public virtual void Hold() { }
        public virtual void Release() { }

        public virtual void Clear() {

        }

        protected void UpdateActionStatus(ActionStatus status) {
            Status = status;
            OnActionStatusUpdated?.Invoke(this, status);
        }
    }

    public struct CharacterActionResponse {
        public bool Success;
        public bool Bufferable;
        public ICharacterActionState State;

        public CharacterActionResponse(bool success, bool bufferable, ICharacterActionState state) {
            Success = success;
            Bufferable = bufferable;
            State = state;
        }
    }
}
