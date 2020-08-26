using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/3D/Attack3D")]
    public class AttackActionData3D : CharacterActionData {

        [SerializeField] private List<CombatHitboxDataEntry3D> _combatHitBoxDatas = new List<CombatHitboxDataEntry3D>();

        // for auto-correcting the character for the attack angle
        [SerializeField] private List<SubActionData> _onActionStartSubActions;

        public IReadOnlyList<CombatHitboxDataEntry3D> CombatHitboxDatas => _combatHitBoxDatas;
        public IReadOnlyList<SubActionData> OnActionStartSubActions => _onActionStartSubActions;

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
            // perform initial sub actions
            PerformActionStartSubActions();
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos() {
            for (int i = 0; i < _data.CombatHitboxDatas.Count; i++) {
                // add the hitbox by id to reference in case a hit is triggered
                CombatHitBoxData modifierInfo = _data.CombatHitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(_data.CombatHitboxDatas[i].Id, modifierInfo);
                // initialize the hitbox with a combat info
                CombatHitboxInfo3D newInfo = new CombatHitboxInfo3D(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(_data.CombatHitboxDatas[i].Id, newInfo);
            }
        }

        private void PerformActionStartSubActions() {
            for(int i = 0; i < _data.OnActionStartSubActions.Count; i++) {
                SubActionData subAction = _data.OnActionStartSubActions[i];
                subAction.PerformAction(subAction.CreateInitData(_character));
            }
        }

        private void OnHitboxTriggered(Hitbox hitBox, Collider collider) {
            // if failed to retrieve hitbox data
            if (!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData3D), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            // create and set the hit event info
            // find out what type of thing we hit
            Hurtbox3D hurtBox = collider.GetComponent<Hurtbox3D>();
            // if the thing we hit was not a hurtbox
            if (hurtBox == null) {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null && damageable != _character.Damageable) {
                    OnDamageableHit(damageable);
                }
                Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    rigidbody.AddForce(_hitEventInfo.KnockBackDirection * _hitEventInfo.Force, ForceMode.Impulse);
                }
                return;
            }
            // ensure this isn't the character's own hitbox
            List<Hurtbox> characterHurtBoxes = new List<Hurtbox>(_character.HurtboxController.Hurtboxes.Values);
            if (characterHurtBoxes.Contains(hurtBox)) {
                return;
            }
            SetHitEventInfo(hitBoxData);
            hurtBox.SendHitEvent(_character, hitBox, OnCharacterHitSuccess);
        }

        // sets the hit event info for this attack state
        private void SetHitEventInfo(CombatHitBoxData hitBoxData) {
            int power = CalculatePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(_character.MoveController.Body, hitBoxData.KnockbackAngle);
            _hitEventInfo = new HitEventInfo() {
                Power = power,
                KnockBackDirection = direction,
                Force = hitBoxData.KnockbackForce,
                OverrideForce = hitBoxData.OverrideForce,
                Attacker = _character
            };
        }

        // upon hitting a character
        private void OnCharacterHitSuccess(ICharacter otherCharacter) {
            if(otherCharacter == null) {
                return;
            }
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

