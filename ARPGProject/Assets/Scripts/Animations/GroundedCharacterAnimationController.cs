using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public abstract class GroundedCharacterAnimationController : MonoBehaviour, IAnimationController {

        protected const string MoveKey = "Move";
        protected const string AirborneKey = "Airborne";

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action<AnimationState> OnAnimationStateUpdated;
        public event Action<string> OnAnimationMessageSent;

        [SerializeField] protected EquipmentManager _equipmentManager;
        [SerializeField] protected Animator _animator;

        protected AnimatorOverrideController _overrideController;
        protected AnimationClipOverrides _overrides;
        protected bool _enabled = true;

        public void UpdateAnimationState(AnimationData data) {
            if (data == null) {
                return;
            }
            if (!string.IsNullOrEmpty(data.AnimationName)) {
                _animator.Play(data.AnimationName);
            }
            for (int i = 0; i < data.Triggers.Length; i++) {
                _animator.SetTrigger(data.Triggers[i]);
            }
            for (int i = 0; i < data.ResetTriggers.Length; i++) {
                _animator.ResetTrigger(data.Triggers[i]);
            }
            for (int i = 0; i < data.IntValues.Length; i++) {
                _animator.SetInteger(data.IntValues[i].Key, data.IntValues[i].Value);
            }
            for (int i = 0; i < data.FloatValues.Length; i++) {
                _animator.SetFloat(data.FloatValues[i].Key, data.FloatValues[i].Value);
            }
            for (int i = 0; i < data.BoolValues.Length; i++) {
                _animator.SetBool(data.BoolValues[i].Key, data.BoolValues[i].Value);
            }
        }

        protected void Start() {
            if(_equipmentManager != null) {
                _equipmentManager.OnEquipmentUpdated += OnEquipmentUpdated;
            }
            SetupOverrideController();
        }

        protected void Update() {
            ProcessMove();
            ProcessAirState();
        }

        protected void SetupOverrideController() {
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
            _overrides = new AnimationClipOverrides(_overrideController.overridesCount);
            _overrideController.GetOverrides(_overrides);
        }

        protected void UpdateActionStatus(ActionStatus status) {
            OnActionStatusUpdated?.Invoke(status);
        }

        protected void SendAnimationEvent(AnimationState state) {
            OnAnimationStateUpdated?.Invoke(state);
        }

        protected void SendAnimationMessage(string message) {
            OnAnimationMessageSent?.Invoke(message);
        }

        protected void OnEquipmentUpdated(IEquipment equipment) {
            IReadOnlyList<AnimationClipOverride> overrideClips = equipment?.AnimationOverrides ?? new List<AnimationClipOverride>();
            OverrideAnimationController(equipment.AnimationOverrides);
        }

        public void OverrideAnimationController(IReadOnlyList<AnimationClipOverride> overrideClips) {
            for (int i = 0; i < overrideClips.Count; i++) {
                _overrides[overrideClips[i].Id] = overrideClips[i].AnimationClip;
            }
            _overrideController.ApplyOverrides(_overrides);
        }

        public AnimatorClipInfo[] GetCurrentAnimatorClipInfos() {
            return _animator.GetCurrentAnimatorClipInfo(0);
        }

        protected abstract void ProcessMove();

        protected abstract void ProcessAirState();
    }
}
