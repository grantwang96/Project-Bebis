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

        ICharacterActionStateV2 GenerateCharacterActionState(ICharacterV2 character);
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

        public virtual ICharacterActionStateV2 GenerateCharacterActionState(ICharacterV2 character) {
            return CreateActionState(character);
        }

        protected abstract ICharacterActionStateV2 CreateActionState(ICharacterV2 character);
    }

    public interface ICharacterActionStateV2
    {
        ICharacterActionDataV2 Data { get; }
        ActionStatus Status { get; }
        bool CanInterrupt(CharacterActionContext context, ICharacterActionDataV2 data);
        bool CanPerform(CharacterActionContext context);
        bool TryPerformAction(CharacterActionContext context);

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
            return (Status == ActionStatus.Ready || CanTransition()) && (Data.AllowedTransitionMoves & data.Tags) != 0;
        }

        public virtual bool CanPerform(CharacterActionContext context) {
            return (Status == ActionStatus.Ready || CanTransition()) && (Data.ActionableContexts & context) != 0;
        }

        public virtual bool TryPerformAction(CharacterActionContext context) {
            if (_character.ActionController.CurrentState?.CanInterrupt(context, Data) ?? true &&
                CanPerform(context)) {
                OnPerformAction(context);
                return true;
            }
            return false;
        }

        protected virtual void OnPerformAction(CharacterActionContext context) {
            Debug.Log($"[{_character.GameObject.name}]: Performing action {Data.Id}...");
        }

        public virtual void Interrupt() {
            Clear();
        }

        public virtual void Clear() {
            Status = ActionStatus.Ready;
        }

        protected virtual bool CanTransition() {
            return Status == ActionStatus.CanTransition || Status == ActionStatus.Completed;
        }

        protected virtual void UpdateActionStatus(ActionStatus status) {
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

