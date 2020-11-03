using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Winston {
    public abstract class AbstractState : MonoBehaviour {
        public AbstractState ParentState { get; protected set; }
        public string StateId { get; protected set; }
        public bool Initialized { get; protected set; }
        public bool Active => _active;

        public event Action<bool> OnStateEntered;
        public event Action<bool> OnStateExited;

        [SerializeField] protected List<AbstractStateTransition> _transitionEntries = new List<AbstractStateTransition>();
        [SerializeField] protected bool _active;
        protected readonly Dictionary<string, AbstractState> _transitions = new Dictionary<string, AbstractState>();

        private void Awake() {
            InitializeState();
        }

        public void InitializeState() {
            // get parent state
            ParentState = transform.parent?.GetComponent<AbstractState>();
            // set up parent state (assuming it's not set up yet)
            if (ParentState != null && !ParentState.Initialized) {
                ParentState.InitializeState();
            }
            if (Initialized) {
                return;
            }
            // do personal state set up
            // ...
            InitializeTransitions();
            InitializeStateId();
            Initialized = true;
        }

        private void InitializeTransitions() {
            for (int i = 0; i < _transitionEntries.Count; i++) {
                _transitions.Add(_transitionEntries[i].TransitionId, _transitionEntries[i].State);
            }
        }

        private void InitializeStateId() {
            StateId = name;
            if (ParentState != null) {
                StateId = $"{ParentState.StateId}/{StateId}";
            }
        }

        // attempt to enter this state
        public virtual void TryEnter() {
            // if already active, no need to re-enter
            if (Active) {
                return;
            }
            // if we have a parent, attempt to enter the parent first and wait until it is finished
            if (!IsReadyToEnter()) {
                return;
            }
            // proceed to enter the state
            Enter();
        }

        // clean up the state
        public virtual void TryExit(AbstractState nextState) {
            // check if we actually need to exit
            if (nextState.IsStateOnPath(this)) {
                return;
            }
            // do personal exit stuff here
            ParentState?.TryExit(nextState);
            Exit();
        }

        public bool IsStateOnPath(AbstractState otherState) {
            // if it is the parent state
            if (ParentState == otherState) {
                return true;
            }
            // if we have no parent state
            if (ParentState == null) {
                return false;
            }
            // check our parent state
            return ParentState.IsStateOnPath(otherState);
        }

        public bool TryGetGameStateFromTransitionId(string id, out AbstractState state) {
            // retrieve game state from own dictionary
            if (_transitions.TryGetValue(id, out state)) {
                return true;
            }
            // retrieve game state from parent's dictionary
            if (ParentState != null) {
                return ParentState.TryGetGameStateFromTransitionId(id, out state);
            }
            return false;
        }

        // set up the state
        protected virtual void Enter() {
            _active = true;
            OnStateEntered?.Invoke(true);
        }

        protected void FailToEnter() {
            OnStateEntered?.Invoke(false);
        }

        protected virtual void Exit() {
            Debug.Log($"[{name}]: Exiting game state {StateId}...");
            _active = false;
            OnStateExited?.Invoke(true);
        }

        protected virtual void FailedToExit() {
            OnStateExited?.Invoke(false);
        }

        protected virtual bool IsReadyToEnter() {
            // if we have a parent, attempt to enter the parent first and wait until it is finished
            if (ParentState != null && !ParentState.Active) {
                ParentState.OnStateEntered += OnParentStateEntered;
                ParentState.TryEnter();
                return false;
            }
            return true;
        }

        // if we're waiting on the parent state to enter
        private void OnParentStateEntered(bool success) {
            ParentState.OnStateEntered -= OnParentStateEntered;
            // if successful, re-try entering the state
            if (success) {
                TryEnter();
            }
        }
    }

}