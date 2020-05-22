using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Attack")]
    public class AttackActionData : CharacterActionData {

        [SerializeField] private List<CombatHitboxDataEntry> _combatHitBoxDatas = new List<CombatHitboxDataEntry>();
        [SerializeField] private bool _applyDash;
        [SerializeField] private float _dashAngle;
        [SerializeField] private float _force;
        [SerializeField] private bool _overrideForce;

        public IReadOnlyList<CombatHitboxDataEntry> CombatHitboxDatas => _combatHitBoxDatas;
        public bool ApplyDash => _applyDash;
        public float DashAngle => _dashAngle;
        public float DashForce => _force;
        public bool OverrideForce => _overrideForce;

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if(state != null && !state.Status.HasFlag(ActionStatus.CanTransition) && !state.Status.HasFlag(ActionStatus.Completed)) {
                return new CharacterActionResponse(false, state);
            }
            AttackActionState newState = new AttackActionState(this, character);
            return new CharacterActionResponse(true, newState);
        }
    }

    [System.Serializable]
    public class HitboxInfoEntry {
        [SerializeField] private string _id;
        [SerializeField] private HitboxInfo _hitBoxInfo;

        public string Id => _id;
        public HitboxInfo HitboxInfo => _hitBoxInfo;
    }

    [System.Serializable]
    public class CombatHitboxDataEntry {
        [SerializeField] private string _id;
        [SerializeField] private CombatHitBoxData _combatHitboxData;

        public string Id => _id;
        public CombatHitBoxData CombatHitboxData => _combatHitboxData;
    }

    public class AttackActionState : CharacterActionState {
        
        private AttackActionData _data;
        private Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();

        public AttackActionState(AttackActionData data, ICharacter character) : base(data, character) {
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
                CombatHitboxInfo newInfo = new CombatHitboxInfo(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(_data.CombatHitboxDatas[i].Id, newInfo);
            }
        }

        private void OnHitboxTriggered(Hitbox hitBox, Hurtbox hurtBox) {
            if(!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            int power = GeneratePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(hitBox.transform, hitBoxData.KnockbackAngle);
            hurtBox.SendHitEvent(
                new HitEventInfo(hitBox, power, direction, hitBoxData.KnockbackForce));
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
