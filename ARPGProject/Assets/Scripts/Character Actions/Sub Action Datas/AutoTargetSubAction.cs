using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Sub Action Data/AutoTargetSubAction")]
    public class AutoTargetSubAction : SubActionData {

        [SerializeField] private float _scanAngle;
        [SerializeField] private float _scanRange;
        [SerializeField] private LayerMask _scanLayers;

        public override SubActionInitializationData CreateInitData(ICharacter user) {
            return new AutoTargetInitData() {
                Character = user,
                Data = this
            };
        }

        public override void PerformAction(SubActionInitializationData subActionInitData) {
            AutoTargetInitData initData = subActionInitData as AutoTargetInitData;
            if(initData == null) {
                CustomLogger.Error(nameof(AutoTargetSubAction), $"Did not receive init data of type {nameof(AutoTargetInitData)}");
                return;
            }
            TryAutoCorrectForTarget(initData);
        }

        private void TryAutoCorrectForTarget(AutoTargetInitData initData) {
            ICharacter target = null;
            ICharacter character = initData.Character;
            if (character.TargetManager.CurrentTarget != null &&
                TargetWithinFieldOfView(character, character.TargetManager.CurrentTarget, out float yeet) &&
                TargetVisible(character, character.TargetManager.CurrentTarget)) {
                target = character.TargetManager.CurrentTarget;
            }
            // if there is no target or not in range
            if (target == null) {
                float bestAngle = -1f;
                Collider[] possibleTargets = Physics.OverlapSphere(
                    initData.Character.MoveController.Center.position,
                    _scanRange,
                    _scanLayers);
                for (int i = 0; i < possibleTargets.Length; i++) {
                    ICharacter otherCharacter = possibleTargets[i].GetComponent<ICharacter>();
                    // ignore if this is not a valid target
                    if (!IsValidTarget(character, otherCharacter)) {
                        continue;
                    }
                    // TODO: ADD HOSTILE TAGS CHECK HERE
                    float angle = 0f;
                    if (TargetWithinFieldOfView(character, otherCharacter, out angle) && TargetVisible(character, otherCharacter)) {
                        if (target == null || angle < bestAngle) {
                            bestAngle = angle;
                            target = otherCharacter;
                            TryOverrideCurrentTarget(character, target);
                        }
                    }
                }
            }
            // if a target has been found
            if (target != null) {
                AutoCorrectForTarget(character, target);
            }
        }

        private bool IsValidTarget(ICharacter character, ICharacter target) {
            return target != null &&
                target != character &&
                !target.Damageable.Dead;
        }

        private void TryOverrideCurrentTarget(ICharacter character, ICharacter target) {
            character.TargetManager.OverrideCurrentTarget(target);
        }

        private bool TargetWithinFieldOfView(ICharacter character, ICharacter target, out float angle) {
            Vector3 direction = (target.MoveController.Body.position - character.MoveController.Body.position).normalized;
            angle = Vector3.Angle(character.MoveController.Body.forward, direction);
            return angle <= _scanAngle;
        }

        private bool TargetVisible(ICharacter character, ICharacter target) {
            Vector3 direction = (target.MoveController.Center.position - character.MoveController.Center.position).normalized;
            if (Physics.Raycast(
                character.MoveController.Center.position, direction,
                out RaycastHit info,
                _scanRange,
                _scanLayers,
                QueryTriggerInteraction.Ignore)) {
                Collider collider = info.collider;
                ICharacter hitCharacter = collider.GetComponent<ICharacter>();
                if (hitCharacter == target) {
                    return true;
                }
            }
            return false;
        }

        private void AutoCorrectForTarget(ICharacter character, ICharacter target) {
            Vector3 direction = target.MoveController.Body.position - character.MoveController.Body.position;
            direction.y = 0f;
            character.MoveController.OverrideRotation(direction.normalized);
        }

    }

    public class AutoTargetInitData : SubActionInitializationData {
        public ICharacter Character;
    }
}
