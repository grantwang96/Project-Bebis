using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    // base class for AI states
    public abstract class AIState : CharacterComponent {
        
        [SerializeField] private AnimationData _animationData;

        protected AIState _parentState;
        public bool Active { get; protected set; }
        
        public event Action<AIState> OnReadyToChangeState;

        protected override void Awake() {
            base.Awake();
            _parentState = transform.parent?.GetComponent<AIState>();
        }

        public virtual void Enter() {
            // if already active, no need to re-enter
            if (Active) {
                return;
            }
            _character.AnimationController?.UpdateAnimationState(_animationData);
            if(_parentState != null) {
                _parentState.Enter();
                _parentState.OnReadyToChangeState += FireReadyToChangeState;
            }
            Active = true;
        }

        public virtual void Execute() {
            _parentState?.Execute();
        }

        public virtual void Exit(AIState nextState) {
            // is this state on the next state's path?
            if (nextState.IsStateOnPath(this)) {
                // if so, no need to exit this state
                return;
            }
            if(_parentState != null) {
                _parentState.Exit(nextState);
                _parentState.OnReadyToChangeState -= FireReadyToChangeState;
            }
            Active = false;
        }

        protected void FireReadyToChangeState(AIState state) {
            OnReadyToChangeState?.Invoke(state);
        }

        public bool IsStateOnPath(AIState state) {
            // is this the state?
            if(state == this) {
                return true;
            }
            // have we exhausted all parents?
            if(_parentState == null) {
                return false;
            }
            // ask the parent
            return _parentState.IsStateOnPath(state);
        }
    }

    public class AIStateInitializationData {

    }
}
