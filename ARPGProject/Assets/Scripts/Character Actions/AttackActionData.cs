using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Attack")]
    public class AttackActionData : CharacterActionData {

        [SerializeField] private AnimationData _animationData;
        [SerializeField] private bool _cancelable;

        [SerializeField] private List<HitboxModifierEntry> _hitBoxModifierData = new List<HitboxModifierEntry>();
        [SerializeField] private bool _applyDash;
        [SerializeField] [Range(0f, 360f)] private float _dashDirection;
        [SerializeField] private float _force;
        [SerializeField] private bool _overrideForce;

        public AnimationData AnimationData => _animationData;
        public bool Cancelable => _cancelable;

        public IReadOnlyList<HitboxModifierEntry> HitboxModifierData => _hitBoxModifierData;
        public bool ApplyDash => _applyDash;
        public float DashDirection => _dashDirection;
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

    public class AttackActionState : ICharacterActionState {

        public CharacterActionData Data => _data;
        public AnimationData AnimationData => _data.AnimationData;
        public bool Cancelable => _data.Cancelable;
        public ActionPermissions Permissions => _data.Permissions;
        public ActionStatus Status { get; set; }

        private ICharacter _character;
        private AttackActionData _data;

        public AttackActionState(AttackActionData data, ICharacter character) {
            // initialize data
            _data = data;
            Status = ActionStatus.Started;
            
            // hook into character events
            _character = character;
            _character.ActionController.OnActionStatusUpdated += OnActionStatusUpdated;

            // update hitboxes
            SetHitboxInfos();
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos() {
            for(int i = 0; i < _data.HitboxModifierData.Count; i++) {
                HitboxInfo info = new HitboxInfo(
                    _data.HitboxModifierData[i].HitboxModifierInfo.BasePower,
                    _data.HitboxModifierData[i].HitboxModifierInfo.KnockbackDirection,
                    _data.HitboxModifierData[i].HitboxModifierInfo.KnockbackForce);
                _character.HitboxController.SetHitboxInfo(_data.HitboxModifierData[i].Id, info);
            }
        }

        // unsubscribe to character events
        public void Clear() {
            _character.ActionController.OnActionStatusUpdated -= OnActionStatusUpdated;
        }

        private void OnActionStatusUpdated(ActionStatus status) {
            // do action status update here
            if(status == ActionStatus.InProgress) {
                TryDash();
            }
            if(status == ActionStatus.Completed) {
                Clear();
            }
        }

        // attempt to force the character in a direction
        private void TryDash() {
            if (!_data.ApplyDash) {
                return;
            }
            Vector2 direction = GetRelativeDirection().normalized;
            _character.MoveController.AddForce(direction, _data.DashForce, _data.OverrideForce);
        }

        // Calculate the general direction that the player will move
        private Vector2 GetRelativeDirection() {
            Vector2 lookDir = _character.MoveController.Rotation;
            float sin = Mathf.Sin(_data.DashDirection * Mathf.Deg2Rad);
            float cos = Mathf.Cos(_data.DashDirection * Mathf.Deg2Rad);
            float tx = lookDir.x;
            float ty = lookDir.y;
            lookDir.x = (cos * tx) - (sin * ty);
            lookDir.y = (sin * tx) + (cos * ty);
            return lookDir;
        }
    }
}
