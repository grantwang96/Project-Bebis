using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Sub Action Data V2/AutoTargetSubAction")]
    public class AutoTargetSubActionV2 : SubActionDataV2
    {
        [SerializeField] private float _scanAngle;
        [SerializeField] private float _scanRange;
        [SerializeField] private LayerMask _scanLayers;

        public override SubActionInitializationDataV2 CreateInitData(ICharacterV2 user) {
            return new AutoTargetInitDataV2() {
                Character = user,
                Data = this
            };
        }

        public override void PerformAction(SubActionInitializationDataV2 subActionInitData) {
            AutoTargetInitDataV2 initData = subActionInitData as AutoTargetInitDataV2;
            if (initData == null) {
                CustomLogger.Error(nameof(AutoTargetSubAction), $"Did not receive init data of type {nameof(AutoTargetInitData)}");
                return;
            }
            TryAutoCorrectForTarget(initData);
        }

        private void TryAutoCorrectForTarget(AutoTargetInitDataV2 initData) {
            ICharacterV2 target = null;
            ICharacterV2 character = initData.Character;
            // attempt to target the designated target
            if (character.UnitController.TargetManager.CurrentTarget != null &&
                TargetWithinFieldOfView(character, character.UnitController.TargetManager.CurrentTarget, out float yeet) &&
                TargetVisible(character, character.UnitController.TargetManager.CurrentTarget)) {
                target = character.UnitController.TargetManager.CurrentTarget;
            }
            // if there is no target or not in range
            if (target == null) {
                float bestAngle = -1f;
                Collider[] possibleTargets = Physics.OverlapSphere(
                    initData.Character.Center.position,
                    _scanRange,
                    _scanLayers);
                for (int i = 0; i < possibleTargets.Length; i++) {
                    ICharacterV2 otherCharacter = possibleTargets[i].GetComponent<ICharacterV2>();
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
                           //  TryOverrideCurrentTarget(character, target);
                        }
                    }
                }
            }
            // if a target has been found
            if (target != null) {
                AutoCorrectForTarget(character, target);
            }
        }

        private bool IsValidTarget(ICharacterV2 character, ICharacterV2 target) {
            return target != null &&
                target != character &&
                !target.Damageable.IsDead;
        }

        private void TryOverrideCurrentTarget(ICharacterV2 character, ICharacterV2 target) {
            // character.TargetManager.OverrideCurrentTarget(target);
        }

        private bool TargetWithinFieldOfView(ICharacterV2 character, ICharacterV2 target, out float angle) {
            Vector3 direction = (target.Center.position - character.GameObject.transform.position).normalized;
            angle = Vector3.Angle(character.GameObject.transform.forward, direction);
            return angle <= _scanAngle;
        }

        private bool TargetVisible(ICharacterV2 character, ICharacterV2 target) {
            Vector3 direction = (target.Center.position - character.Center.position).normalized;
            if (Physics.Raycast(
                character.Center.position, direction,
                out RaycastHit info,
                _scanRange,
                _scanLayers,
                QueryTriggerInteraction.Ignore)) {
                Collider collider = info.collider;
                ICharacterV2 hitCharacter = collider.GetComponent<ICharacterV2>();
                if (hitCharacter == target) {
                    return true;
                }
            }
            return false;
        }

        private void AutoCorrectForTarget(ICharacterV2 character, ICharacterV2 target) {
            Vector3 direction = target.GameObject.transform.position - character.GameObject.transform.position;
            direction.y = 0f;
            character.MoveController.OverrideRotation(direction.normalized);
        }
    }

    public class AutoTargetInitDataV2 : SubActionInitializationDataV2
    {
        public ICharacterV2 Character;
    }
}
