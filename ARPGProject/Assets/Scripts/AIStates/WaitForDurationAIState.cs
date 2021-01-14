using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class WaitForDurationAIState : AIStateV2
    {
        public const string Name = "WaitForDuration";
        private const string MinimumWaitTimeId = "MinimumWaitTime";
        private const string MaximumWaitTimeId = "MaximumWaitTime";
        private const string OnWaitCompletedId = "OnWaitCompleted";

        private IAIState _onWaitCompleted;
        private float _minimumWaitTime = 0f;
        private float _maximumWaitTime = 0f;
        private float _currentDuration;
        private float _targetDuration;

        public WaitForDurationAIState(ICharacterV2 character, AIStateMachineV2 stateMachine, AIStateData stateData, IAIState parent = null) : 
            base(character, stateMachine, stateData, parent) {

        }

        public override void Initialize() {
            base.Initialize();
            SetWaitTimes();
            SetOnWaitCompleted();
        }

        protected override void OnEnter() {
            base.OnEnter();
            MonoBehaviourMaster.Instance.OnUpdate += OnUpdate;
            _currentDuration = 0f;
            _targetDuration = Random.Range(_minimumWaitTime, _maximumWaitTime);
        }

        protected override void OnExit() {
            base.OnExit();
            MonoBehaviourMaster.Instance.OnUpdate -= OnUpdate;
        }

        private void SetWaitTimes() {
            // set the minimum wait time
            int index = _stateData.FloatDatas.FindIndex(x => x.Key.Equals(MinimumWaitTimeId));
            if (index != -1) {
                _minimumWaitTime = _stateData.FloatDatas[index].Value;
            }
            // set the maximum wait time
            index = _stateData.FloatDatas.FindIndex(x => x.Key.Equals(MaximumWaitTimeId));
            if (index != -1) {
                _maximumWaitTime = _stateData.FloatDatas[index].Value;
            }
        }

        private void SetOnWaitCompleted() {
            // attempt to find the corresponding transition id
            int index = _stateData.TransitionDatas.FindIndex(x => x.Key.Equals(OnWaitCompletedId));
            if (index == -1) {
                Debug.LogError($"[{Id}]: Could not retrieve \'{OnWaitCompletedId}\' transition id from state data {_stateName}");
                return;
            }
            // attempt to retrieve the corresponding state for "On Hit"
            if (!_stateMachine.TryGetAIState(_stateData.TransitionDatas[index].Value, out _onWaitCompleted)) {
                Debug.LogError($"[{Id}]: Could not retreive state with Id {_stateData.TransitionDatas[index].Value} from state machine!");
            }
        }

        private void OnUpdate() {
            _currentDuration += Time.deltaTime;
            if(_currentDuration >= _targetDuration) {
                FireReadyToTransition(_onWaitCompleted);
            }
        }
    }
}
