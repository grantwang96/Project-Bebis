using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    /// <summary>
    /// Tags that generally describes the move
    /// </summary>
    [System.Flags]
    public enum CharacterActionTag
    {
        Interact = 1 << 0,
        Jump = 1 << 1,
        Defend = 1 << 2,
        NormalAttack = 1 << 3,
        SecondaryAttack = 1 << 4,
        Grab = 1 << 5,
        SpecialAttack = 1 << 6,
        Buff = 1 << 7,
        CommandGrab = 1 << 8
    }

    /// <summary>
    /// interface for working with a character action as data
    /// </summary>
    public interface ICharacterActionDataV2
    {
        string Id { get; } // the specific id for this action data
        int Priority { get; }
        CharacterActionTag Tags { get; }
        CharacterActionTag AllowedTransitionMoves { get; }
        CharacterActionContext ActionableContexts { get; }

        CharacterActionResponseV2 Initiate(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
        CharacterActionResponseV2 Hold(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
        CharacterActionResponseV2 Release(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context);
    }

    /// <summary>
    /// Base scriptable object for character actions
    /// </summary>
    public abstract class CharacterActionDataV2 : ScriptableObject, ICharacterActionDataV2
    {
        [SerializeField] private string _id;
        [SerializeField] private string _actionName;
        [SerializeField] private CharacterActionTag _tags;
        [SerializeField] private CharacterActionTag _allowedTransitionMoves;

        [SerializeField] protected bool _cancelable;
        [SerializeField] protected int _priority;
        [SerializeField] protected bool _bufferable;
        [SerializeField] protected CharacterActionContext _actionableContexts;

        public string Id => _id;
        public string ActionName => _actionName;
        public bool Cancelable => _cancelable;
        public int Priority => _priority;
        public bool Bufferable => _bufferable;
        public CharacterActionContext ActionableContexts => _actionableContexts;
        public CharacterActionTag Tags => _tags;
        public CharacterActionTag AllowedTransitionMoves => _allowedTransitionMoves;

        public virtual CharacterActionResponseV2 Initiate(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState, context)) {
                return FailedActionResponse(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = HandleActionSuccess(character, foundActionState);
            response.State.Initiate();
            return response;
        }

        public virtual CharacterActionResponseV2 Hold(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState, context)) {
                return FailedActionResponse(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = HandleActionSuccess(character, foundActionState);
            response.State.Hold();
            return response;
        }

        public virtual CharacterActionResponseV2 Release(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            CharacterActionResponseV2 response = new CharacterActionResponseV2();
            // exit early if there is a current action and it has priority
            if (!CanPerformAction(character, foundActionState, context)) {
                return FailedActionResponse(character);
            }
            // create a successful action use response
            response.Success = true;
            response.State = HandleActionSuccess(character, foundActionState);
            response.State.Release();
            return response;
        }

        protected virtual bool CanPerformAction(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            if (!_actionableContexts.HasFlag(context)) {
                return false;
            }
            ICharacterActionStateV2 currentAction = character.ActionController.CurrentState;
            bool canPerform = currentAction?.CanInterrupt(context, this) ?? true;
            canPerform &= foundActionState?.CanPerform(context, this) ?? true;
            return canPerform;
        }

        protected CharacterActionResponseV2 FailedActionResponse(ICharacterV2 character) {
            return new CharacterActionResponseV2(false, false, character.ActionController.CurrentState);
        }

        protected abstract ICharacterActionStateV2 HandleActionSuccess(ICharacterV2 character, ICharacterActionStateV2 foundActionState);

        protected abstract ICharacterActionStateV2 CreateActionState(ICharacterV2 character);
    }

    public interface ICharacterActionStateV2
    {
        ICharacterActionDataV2 Data { get; }
        ActionStatus Status { get; }
        bool CanInterrupt(CharacterActionContext context, ICharacterActionDataV2 data);
        bool CanPerform(CharacterActionContext context, ICharacterActionDataV2 data);
        void Initiate();
        void Hold();
        void Release();

        event Action<ICharacterActionStateV2, ActionStatus> OnActionStatusUpdated;

        void Clear();
        void Interrupt();
    }

    public class CharacterActionStateV2 : ICharacterActionStateV2
    {
        public ICharacterActionDataV2 Data { get; }
        public ActionStatus Status { get; protected set; }

        public event Action<ICharacterActionStateV2, ActionStatus> OnActionStatusUpdated;

        protected ICharacterV2 _character;

        public CharacterActionStateV2(ICharacterActionDataV2 data, ICharacterV2 character) {
            Data = data;
            _character = character;
        }

        public virtual bool CanInterrupt(CharacterActionContext context, ICharacterActionDataV2 data) {
            return CanTransition() && (Data.AllowedTransitionMoves & data.Tags) != 0;
        }

        public virtual bool CanPerform(CharacterActionContext context, ICharacterActionDataV2 data) {
            return CanTransition() && (Data.ActionableContexts & context) != 0;
        }

        public virtual void Initiate() { }
        public virtual void Hold() { }
        public virtual void Release() { }
        public virtual void Interrupt() {
            Clear();
        }

        public virtual void Clear() {
            UpdateActionStatus(ActionStatus.Completed);
        }

        protected virtual bool CanTransition() {
            return Status == ActionStatus.CanTransition || Status == ActionStatus.Completed;
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

