﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/2D/Attack2D")]
    public class AttackActionData2D : CharacterActionData {

        [SerializeField] private List<CombatHitboxDataEntry2D> _combatHitBoxDatas = new List<CombatHitboxDataEntry2D>();
        [SerializeField] private bool _applyDash;
        [SerializeField] private float _dashAngle;
        [SerializeField] private float _force;
        [SerializeField] private bool _overrideForce;

        public IReadOnlyList<CombatHitboxDataEntry2D> CombatHitboxDatas => _combatHitBoxDatas;
        public bool ApplyDash => _applyDash;
        public float DashAngle => _dashAngle;
        public float DashForce => _force;
        public bool OverrideForce => _overrideForce;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if(state != null && !state.Status.HasFlag(ActionStatus.CanTransition) && !state.Status.HasFlag(ActionStatus.Completed)) {
                return new CharacterActionResponse(false, state);
            }
            AttackActionState2D newState = new AttackActionState2D(this, character);
            return new CharacterActionResponse(true, newState);
        }
    }

    [System.Serializable]
    public class HitboxInfoEntry2D {
        [SerializeField] private string _id;
        [SerializeField] private HitboxInfo2D _hitBoxInfo;

        public string Id => _id;
        public HitboxInfo2D HitboxInfo => _hitBoxInfo;
    }

    [System.Serializable]
    public class CombatHitboxDataEntry2D {
        [SerializeField] private string _id;
        [SerializeField] private CombatHitBoxData _combatHitboxData;

        public string Id => _id;
        public CombatHitBoxData CombatHitboxData => _combatHitboxData;
    }

    public class AttackActionState2D : CharacterActionState {
        
        private AttackActionData2D _data;
        private Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();

        private HitEventInfo _hitEventInfo;

        public AttackActionState2D(AttackActionData2D data, ICharacter character) : base(data, character) {
            // initialize data
            _data = data;
            Status = ActionStatus.Started;

            // update hitboxes
            SetHitboxInfos();
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos() {
            for(int i = 0; i < _data.CombatHitboxDatas.Count; i++) {
                CombatHitBoxData modifierInfo = _data.CombatHitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(_data.CombatHitboxDatas[i].Id, modifierInfo);
                CombatHitboxInfo2D newInfo = new CombatHitboxInfo2D(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(_data.CombatHitboxDatas[i].Id, newInfo);
            }
        }

        private void OnHitboxTriggered(Hitbox hitBox, Collider2D collider) {
            if(!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData2D), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            Hurtbox2D hurtBox = collider.GetComponent<Hurtbox2D>();
            if(hurtBox == null) {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if(damageable != null) {
                    OnDamageableHit(damageable);
                }
                return;
            }
            if (_character.HurtboxController.Hurtboxes.ContainsKey(hurtBox.name)) {
                return;
            }
            int power = GeneratePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(hitBox.transform, hitBoxData.KnockbackAngle.z);
            _hitEventInfo = new HitEventInfo(power, direction, hitBoxData.KnockbackForce);
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

        private Vector3 CalculateRelativeDirection(Transform transform, float angle) {
            return ExtraMath.Rotate(transform.up, angle);
        }

        protected override void OnActionStatusUpdated(ActionStatus status) {
            // do action status update here
            if(status == ActionStatus.InProgress) {
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
        private Vector2 GetRelativeDirection(float angle) {
            return ExtraMath.Rotate(_character.MoveController.Rotation, angle);
        }
    }
}