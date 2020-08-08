﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCAnimationController3D : CharacterComponent, IAnimationController {

        public event Action<ActionStatus> OnActionStatusUpdated;
        public event Action<AnimationState> OnAnimationStateUpdated;

        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private Animator _animator;

        private AnimatorOverrideController _overrideController;
        private AnimationClipOverrides _overrides;
        private bool _enabled = true;

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

        private void Start() {
            // _equipmentManager.OnEquipmentUpdated += OnEquipmentUpdated;

            SetupOverrideController();
        }

        private void Update() {
            ProcessMove();
            _animator.SetBool("Airborne", !_character.MoveController.IsGrounded);
        }

        private void SetupOverrideController() {
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
            _overrides = new AnimationClipOverrides(_overrideController.overridesCount);
            _overrideController.GetOverrides(_overrides);
        }

        private void UpdateActionStatus(ActionStatus status) {
            OnActionStatusUpdated?.Invoke(status);
        }

        private void SendAnimationEvent(AnimationState state) {
            OnAnimationStateUpdated?.Invoke(state);
        }

        private void OnEquipmentUpdated(IEquipment equipment) {
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

        public Vector3 DeltaPosition() {
            return _animator.deltaPosition;
        }

        private void ProcessMove() {
            if (!_enabled) {
                return;
            }
            _animator.SetFloat("Move", _character.MoveController.MoveMagnitude);
        }
    }
}
