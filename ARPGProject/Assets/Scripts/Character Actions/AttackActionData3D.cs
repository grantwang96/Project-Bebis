using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/3D/Attack3D")]
    public class AttackActionData3D : CharacterActionData {

        [SerializeField] private List<CombatHitboxDataEntry3D> _combatHitBoxDatas = new List<CombatHitboxDataEntry3D>();

        // for auto-correcting the character for the attack angle
        [SerializeField] private bool _autoCorrectForTarget;
        [SerializeField] private float _scanAngle;
        [SerializeField] private float _scanRange;
        [SerializeField] private LayerMask _scanLayers;

        public IReadOnlyList<CombatHitboxDataEntry3D> CombatHitboxDatas => _combatHitBoxDatas;
        public bool AutoCorrectForTarget => _autoCorrectForTarget;
        public float ScanAngle => _scanAngle;
        public float ScanRange => _scanRange;
        public LayerMask ScanLayers => _scanLayers;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if (!CanAttack(character, state)) {
                bool bufferable = state.Data != this;
                return new CharacterActionResponse(false, bufferable, state);
            }
            AttackActionState3D newState = new AttackActionState3D(this, character);
            return new CharacterActionResponse(true, Bufferable, newState);
        }

        protected virtual bool CanAttack(ICharacter character, ICharacterActionState state) {
            return state == null ||
                state.Status.HasFlag(ActionStatus.CanTransition) ||
                state.Status.HasFlag(ActionStatus.Completed);
        }
    }

    [System.Serializable]
    public class CombatHitboxDataEntry3D {
        [SerializeField] private string _id;
        [SerializeField] private CombatHitBoxData _combatHitboxData;

        public string Id => _id;
        public CombatHitBoxData CombatHitboxData => _combatHitboxData;
    }

    public class AttackActionState3D : CharacterActionState {

        private AttackActionData3D _data;
        private Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();

        private HitEventInfo _hitEventInfo;

        public AttackActionState3D(AttackActionData3D data, ICharacter character) : base(data, character) {
            // initialize data
            _data = data;
            Status = ActionStatus.Started;

            // update hitboxes
            SetHitboxInfos();
            TryAutoCorrectForTarget();
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos() {
            for (int i = 0; i < _data.CombatHitboxDatas.Count; i++) {
                CombatHitBoxData modifierInfo = _data.CombatHitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(_data.CombatHitboxDatas[i].Id, modifierInfo);
                CombatHitboxInfo3D newInfo = new CombatHitboxInfo3D(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(_data.CombatHitboxDatas[i].Id, newInfo);
            }
        }

        private void TryAutoCorrectForTarget() {
            ICharacter target = null;
            if(_character.TargetManager.CurrentTarget != null &&
                TargetWithinFieldOfView(_character.TargetManager.CurrentTarget, out float yeet) &&
                TargetVisible(_character.TargetManager.CurrentTarget)) {
                target = _character.TargetManager.CurrentTarget;
            }
            // if there is no target or not in range
            if(target == null) {
                float bestAngle = -1f;
                Collider[] possibleTargets = Physics.OverlapSphere(_character.MoveController.Center.position, _data.ScanRange, _data.ScanLayers);
                for (int i = 0; i < possibleTargets.Length; i++) {
                    ICharacter otherCharacter = possibleTargets[i].GetComponent<ICharacter>();
                    // ignore if this is not a valid target
                    if (!IsValidTarget(otherCharacter)) {
                        continue;
                    }
                    // TODO: ADD HOSTILE TAGS CHECK HERE
                    float angle = 0f;
                    if (TargetWithinFieldOfView(otherCharacter, out angle) && TargetVisible(otherCharacter)) {
                        if (target == null || angle < bestAngle) {
                            bestAngle = angle;
                            target = otherCharacter;
                            TryOverrideCurrentTarget(target);
                        }
                    }
                }
            }
            // if a target has been found
            if (target != null) {
                AutoCorrectForTarget(target);
            }
        }

        private bool IsValidTarget(ICharacter target) {
            return target != null &&
                target != _character &&
                !target.Damageable.Dead;
        }

        private void TryOverrideCurrentTarget(ICharacter target) {
            _character.TargetManager.OverrideCurrentTarget(target);
        }

        private bool TargetWithinFieldOfView(ICharacter target, out float angle) {
            Vector3 direction = (target.MoveController.Body.position - _character.MoveController.Body.position).normalized;
            angle = Vector3.Angle(_character.MoveController.Body.forward, direction);
            return angle <= _data.ScanAngle;
        }

        private bool TargetVisible(ICharacter target) {
            Vector3 direction = (target.MoveController.Center.position - _character.MoveController.Center.position).normalized;
            if (Physics.Raycast(
                _character.MoveController.Center.position, direction,
                out RaycastHit info,
                _data.ScanRange,
                _data.ScanLayers,
                QueryTriggerInteraction.Ignore)) {
                Collider collider = info.collider;
                ICharacter hitCharacter = collider.GetComponent<ICharacter>();
                if (hitCharacter == target) {
                    return true;
                }
            }
            return false;
        }

        private void AutoCorrectForTarget(ICharacter target) {
            Vector3 direction = target.MoveController.Body.position - _character.MoveController.Body.position;
            direction.y = 0f;
            _character.MoveController.OverrideRotation(direction.normalized);
        }

        private void OnHitboxTriggered(Hitbox hitBox, Collider collider) {
            if (!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData3D), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            SetHitEventInfo(hitBoxData);
            Hurtbox3D hurtBox = collider.GetComponent<Hurtbox3D>();
            if (hurtBox == null) {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null && damageable != _character.Damageable) {
                    OnDamageableHit(damageable);
                }
                return;
            }
            List<Hurtbox> characterHurtBoxes = new List<Hurtbox>(_character.HurtboxController.Hurtboxes.Values);
            if (characterHurtBoxes.Contains(hurtBox)) {
                return;
            }
            hurtBox.SendHitEvent(hitBox, OnCharacterHit);
        }

        // sets the hit event info for this attack state
        private void SetHitEventInfo(CombatHitBoxData hitBoxData) {
            int power = CalculatePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(_character.MoveController.Body, hitBoxData.KnockbackAngle);
            _hitEventInfo = new HitEventInfo(power, direction, hitBoxData.KnockbackForce, hitBoxData.OverrideForce, _character);
        }

        // upon hitting a character
        private void OnCharacterHit(ICharacter otherCharacter) {
            OnDamageableHit(otherCharacter.Damageable);
        }

        // upon hitting a damageable object
        private void OnDamageableHit(IDamageable damageable) {
            damageable.TakeDamage(_hitEventInfo);
        }

        // calculate total damage
        private int CalculatePower(ICharacterStatManager characterStatManager, int basePower, MinMax_Int range) {
            int attackPower = characterStatManager.Attack;
            attackPower += basePower;
            attackPower += Random.Range(range.Min, range.Max);
            return attackPower;
        }

        // calculate knockback direction in worldspace
        private Vector3 CalculateRelativeDirection(Transform transform, Vector3 angle) {
            return transform.TransformDirection(angle);
        }
        
        // Calculate the general direction that the player will move
        private Vector3 GetRelativeDirection(Vector3 angle) {
            return _character.MoveController.Body.TransformDirection(angle);
        }
    }
}

