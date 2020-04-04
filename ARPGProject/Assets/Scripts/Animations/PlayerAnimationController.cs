using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerAnimationController : PlayerCharacterComponent, IAnimationController {

        private EquipmentManager _equipmentManager;
        private Animator _animator;
        private AnimatorOverrideController _overrideController;
        private AnimationClipOverrides _overrides;
        private bool _enabled = true;

        public PlayerAnimationController(PlayerCharacter character, Animator animator, EquipmentManager equipmentManager) : base(character) {
            _animator = animator;
            _character.OnUpdate += OnUpdate;
            _equipmentManager = equipmentManager;
            _equipmentManager.OnEquipmentUpdated += OnEquipmentUpdated;
            SetupOverrideController();
        }

        private void SetupOverrideController() {
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
            _overrides = new AnimationClipOverrides(_overrideController.overridesCount);
            _overrideController.GetOverrides(_overrides);
        }

        public void UpdateAnimationState(AnimationData data) {
            if (!string.IsNullOrEmpty(data.AnimationName)) {
                _animator.Play(data.AnimationName);
            }
            for(int i = 0; i < data.Triggers.Length; i++) {
                _animator.SetTrigger(data.Triggers[i]);
            }
            for(int i = 0; i < data.IntValues.Length; i++) {
                _animator.SetInteger(data.IntValues[i].Key, data.IntValues[i].Value);
            }
            for(int i = 0; i < data.FloatValues.Length; i++) {
                _animator.SetFloat(data.FloatValues[i].Key, data.FloatValues[i].Value);
            }
            for(int i = 0; i < data.BoolValues.Length; i++) {
                _animator.SetBool(data.BoolValues[i].Key, data.BoolValues[i].Value);
            }
        }

        private void OnEquipmentUpdated(IEquipment equipment) {
            IReadOnlyList<AnimationClipOverride> overrideClips = equipment?.AnimationOverrides ?? new List<AnimationClipOverride>();
            OverrideAnimationController(equipment.AnimationOverrides);
        }

        public void OverrideAnimationController(IReadOnlyList<AnimationClipOverride> overrideClips) {
            for(int i = 0; i < overrideClips.Count; i++) {
                _overrides[overrideClips[i].Id] = overrideClips[i].AnimationClip;
            }
            _overrideController.ApplyOverrides(_overrides);
        }

        private void OnUpdate() {
            ProcessMove();
            ProcessLook();
        }

        private void ProcessMove() {
            if (!_enabled) {
                return;
            }
            Vector3 moveVector = _character.MoveController.Move;
            float moveValue = moveVector.magnitude;
            _animator.SetFloat("Move", moveValue);
        }

        private void ProcessLook() {
            if (!_enabled) {
                return;
            }
            int x = GetModifiedAxisInput(_character.MoveController.Move.x);
            int y = GetModifiedAxisInput(_character.MoveController.Move.y);
            if(Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f)) {
                return;
            }
            Vector3 moveVector = _character.MoveController.Move;
            _animator.SetFloat("LookX", x);
            _animator.SetFloat("LookY", y);
        }

        private int GetModifiedAxisInput(float val) {
            if(Mathf.Approximately(val, 0f)) {
                return 0;
            }
            int returnVal = 1;
            if(val < 0f) {
                returnVal *= -1;
            }
            return returnVal;
        }
    }
}
