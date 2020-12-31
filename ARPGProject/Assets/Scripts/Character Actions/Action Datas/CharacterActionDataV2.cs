using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface ICharacterActionDataV2
    {

        string Id { get; }
        int Priority { get; }

        CharacterActionResponseV2 Initiate(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
        CharacterActionResponseV2 Hold(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
        CharacterActionResponseV2 Release(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
    }

    public abstract class CharacterActionDataV2 : ScriptableObject, ICharacterActionDataV2
    {

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

        public virtual CharacterActionResponseV2 Initiate(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            ICharacterActionStateV2 currentAction = character.ActionController.CurrentState;
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
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

        public virtual CharacterActionResponseV2 Hold(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            ICharacterActionStateV2 currentAction = character.ActionController.CurrentState;
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
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

        public virtual CharacterActionResponseV2 Release(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            ICharacterActionStateV2 currentAction = character.ActionController.CurrentState;
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
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

        protected virtual bool CanPerformAction(ICharacterV2 character, ICharacterActionStateV2 foundActionState) {
            return true;
        }

        protected CharacterActionResponseV2 FailedActionResponse(ICharacterV2 character) {
            return new CharacterActionResponseV2(false, false, character.ActionController.CurrentState);
        }

        protected abstract ICharacterActionStateV2 CreateActionState(ICharacterV2 character);
    }

    public interface ICharacterActionStateV2
    {
        ICharacterActionDataV2 Data { get; }
        ActionStatus Status { get; set; }
        void Initiate();
        void Hold();
        void Release();

        event Action<ICharacterActionStateV2, ActionStatus> OnActionStatusUpdated;

        void Clear();
    }

    public class CharacterActionStateV2 : ICharacterActionStateV2
    {

        public ICharacterActionDataV2 Data { get; }
        public ActionStatus Status { get; set; }

        public event Action<ICharacterActionStateV2, ActionStatus> OnActionStatusUpdated;

        protected ICharacterV2 _character;

        public CharacterActionStateV2(ICharacterActionDataV2 data, ICharacterV2 character) {
            Data = data;

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

    public struct CharacterActionResponseV2
    {
        public bool Success;
        public bool Bufferable;
        public ICharacterActionStateV2 State;

        public CharacterActionResponseV2(bool success, bool bufferable, ICharacterActionStateV2 state) {
            Success = success;
            Bufferable = bufferable;
            State = state;
        }
    }
}

