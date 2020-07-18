using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/3D/Attack3D")]
    public class AttackActionData3D : CharacterActionData {

        [SerializeField] private List<CombatHitboxDataEntry3D> _combatHitBoxDatas = new List<CombatHitboxDataEntry3D>();
        [SerializeField] private bool _applyDash;
        [SerializeField] private Vector3 _dashAngle;
        [SerializeField] private float _force;
        [SerializeField] private bool _overrideForce;

        public IReadOnlyList<CombatHitboxDataEntry3D> CombatHitboxDatas => _combatHitBoxDatas;
        public bool ApplyDash => _applyDash;
        public Vector3 DashAngle => _dashAngle;
        public float DashForce => _force;
        public bool OverrideForce => _overrideForce;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if (!CanAttack(character, state)) {
                return new CharacterActionResponse(false, state);
            }
            AttackActionState3D newState = new AttackActionState3D(this, character);
            return new CharacterActionResponse(true, newState);
        }

        protected virtual bool CanAttack(ICharacter character, ICharacterActionState state) {
            return state == null ||
                state.Status.HasFlag(ActionStatus.CanTransition) ||
                state.Status.HasFlag(ActionStatus.Completed);
        }
    }

    [System.Serializable]
    public class HitboxInfoEntry3D {
        [SerializeField] private string _id;
        [SerializeField] private HitboxInfo2D _hitBoxInfo;

        public string Id => _id;
        public HitboxInfo2D HitboxInfo => _hitBoxInfo;
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

        private void OnHitboxTriggered(Hitbox hitBox, Collider collider) {
            if (!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData3D), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            int power = GeneratePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(_character.MoveController.Body, hitBoxData.KnockbackAngle);
            _hitEventInfo = new HitEventInfo(power, direction, hitBoxData.KnockbackForce, _character);
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

        private void OnCharacterHit(ICharacter otherCharacter) {
            OnDamageableHit(otherCharacter.Damageable);
        }

        private void OnDamageableHit(IDamageable damageable) {
            damageable.TakeDamage(_hitEventInfo);
        }

        private int GeneratePower(ICharacterStatManager characterStatManager, int basePower, MinMax_Int range) {
            int attackPower = characterStatManager.Attack;
            attackPower += basePower;
            attackPower += Random.Range(range.Min, range.Max);
            return attackPower;
        }

        private Vector3 CalculateRelativeDirection(Transform transform, Vector3 angle) {
            return transform.TransformDirection(angle);
        }

        protected override void OnActionStatusUpdated(ActionStatus status) {
            // do action status update here
            if (status == ActionStatus.InProgress) {
                TryDash();
            }
            base.OnActionStatusUpdated(status);
        }

        // attempt to force the character in a direction
        private void TryDash() {
            if (!_data.ApplyDash) {
                return;
            }
            Vector3 direction = GetRelativeDirection(_data.DashAngle).normalized;
            _character.MoveController.AddForce(direction, _data.DashForce, _data.OverrideForce);
        }

        // Calculate the general direction that the player will move
        private Vector3 GetRelativeDirection(Vector3 angle) {
            return _character.MoveController.Body.TransformDirection(angle);
        }
    }
}

