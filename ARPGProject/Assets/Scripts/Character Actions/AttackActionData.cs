using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Attack")]
    public class AttackActionData : CharacterActionData {

        [SerializeField] private List<HitboxModifierEntry> _hitBoxModifierData = new List<HitboxModifierEntry>();
        [SerializeField] private bool _applyDash;
        [SerializeField] private float _dashAngle;
        [SerializeField] private float _force;
        [SerializeField] private bool _overrideForce;

        public IReadOnlyList<HitboxModifierEntry> HitboxModifierData => _hitBoxModifierData;
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
    public class HitboxModifierEntry {
        [SerializeField] private string _id;
        [SerializeField] private HitboxModifierInfo _hitboxModifierInfo;

        public string Id => _id;
        public HitboxModifierInfo HitboxModifierInfo => _hitboxModifierInfo;
    }

    public class AttackActionState : CharacterActionState {
        
        private AttackActionData _data;

        public AttackActionState(AttackActionData data, ICharacter character) : base(data, character) {
            // initialize data
            _data = data;
            Status = ActionStatus.Started;

            // update hitboxes
            SetHitboxInfos();
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos() {
            for(int i = 0; i < _data.HitboxModifierData.Count; i++) {
                HitboxModifierInfo modifierInfo = _data.HitboxModifierData[i].HitboxModifierInfo;
                HitboxInfo info = new HitboxInfo(
                    GeneratePower(_character.CharacterStatManager, modifierInfo.BasePower, modifierInfo.PowerRange),
                    _data.HitboxModifierData[i].HitboxModifierInfo.KnockbackAngle,
                    _data.HitboxModifierData[i].HitboxModifierInfo.KnockbackForce);
                _character.HitboxController.SetHitboxInfo(_data.HitboxModifierData[i].Id, info);
            }
        }

        private int GeneratePower(ICharacterStatManager characterStatManager, int basePower, MinMax_Int range) {
            int attackPower = characterStatManager.Attack;
            attackPower += basePower;
            attackPower += Random.Range(range.Min, range.Max);
            return attackPower;
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
            // Vector3 worldDir = _character.MoveController.Body.TransformDirection(_data.DashDirection);
        }
    }
}
