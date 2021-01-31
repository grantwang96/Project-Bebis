using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IAIState
    {
        string Id { get; }
        bool Active { get; }
        bool isSetup { get; }
        event Action<IAIState> OnReadyToTransition;

        void Initialize(ICharacterV2 character);
        void Setup();
        void TryEnter();
        void TryExit(IAIState nextState);
        bool IsStateOnPath(IAIState state);
    }

    public abstract class AIStateV2 : MonoBehaviour, IAIState
    {
        public string Id { get; protected set; }
        public bool Active { get; protected set; }
        public bool isSetup { get; protected set; }

        public event Action<IAIState> OnReadyToTransition;

        protected ICharacterV2 _character;
        [SerializeField] protected AIStateMachineV2 _stateMachine;
        // protected AIStateData _stateData;
        protected IAIState _parentState;

        // initialize is where AI states handle setting up their individual datas and state transitions
        public virtual void Initialize(ICharacterV2 character) {
            _character = character;
            if (transform.parent != null) {
                _parentState = transform.parent.GetComponent<IAIState>();
            }
            Id = name;
        }

        public virtual void Setup() {
            // setup the parent state first
            if(_parentState != null && !_parentState.isSetup) {
                _parentState.Setup();
            }
            SetId();
            _stateMachine?.RegisterAIState(this);
        }

        protected void SetId() {
            if (_parentState != null) {
                Id = $"{_parentState.Id}/{name}";
            } else {
                Id = name;
            }
        }

        public virtual void TryEnter() {
            // if already active
            if (Active) {
                return;
            }
            OnEnter();
        }

        public virtual void TryExit(IAIState nextState) {
            // is this state on the next state's path?
            if (nextState.IsStateOnPath(this)) {
                // if so, no need to exit this state
                return;
            }
            if (_parentState != null) {
                _parentState.TryExit(nextState);
            }
            OnExit();
        }

        public bool IsStateOnPath(IAIState state) {
            // is this the state?
            if (state == this) {
                return true;
            }
            // have we exhausted all parents?
            if (_parentState == null) {
                return false;
            }
            // ask the parent
            return _parentState.IsStateOnPath(state);
        }
        protected virtual void OnEnter() {
            // Debug.Log($"[{_character.GameObject.name}]: Entering state \'{Id}\'...");
            // attempt to enter the parent as well
            if (_parentState != null) {
                if (!_parentState.Active) {
                    _parentState.TryEnter();
                }
                _parentState.OnReadyToTransition += OnParentStateReadyToTransition;
            }
            Active = true;
        }

        protected virtual void OnExit() {
            // Debug.Log($"[{_character.GameObject.name}]: Exiting state \'{Id}\'...");
            if (_parentState != null) {
                _parentState.OnReadyToTransition -= OnParentStateReadyToTransition;
            }
            Active = false;
        }

        protected void OnParentStateReadyToTransition(IAIState nextState) {
            FireReadyToTransition(nextState);
        }

        // allows all AI states to trigger transition
        protected void FireReadyToTransition(IAIState nextState) {
            OnReadyToTransition?.Invoke(nextState);
        }
    }
}
