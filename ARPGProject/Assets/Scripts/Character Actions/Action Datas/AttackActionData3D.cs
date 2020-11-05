using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/3D/Attack3D")]
    public class AttackActionData3D : CharacterActionData {

        [SerializeField] private AttackData _attackData;
        public AttackData AttackData => _attackData;

        protected override bool CanPerformAction(ICharacter character, ICharacterActionState foundActionState) {
            ICharacterActionState currentAction = character.ActionController.CurrentState;
            bool canPerform = currentAction == null;
            if(currentAction != null) {
                canPerform |= currentAction.Status == ActionStatus.CanTransition;
                canPerform |= Priority > currentAction.Data.Priority;
            }
            return canPerform;
        }

        public override CharacterActionResponse Hold(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            return FailedActionResponse(character);
        }

        public override CharacterActionResponse Release(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            return FailedActionResponse(character);
        }

        protected override ICharacterActionState CreateActionState(ICharacter character) {
            return new AttackActionState3D(this, character);
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
        private readonly Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();
        private AnimationData _animationData;

        private HitEventInfo _hitEventInfo;

        public AttackActionState3D(AttackActionData3D data, ICharacter character) : base(data, character) {
            // initialize data
            _data = data;
            Status = ActionStatus.Started;
        }

        public override void Initiate() {
            base.Initiate();
            PerformAttack(_data.AttackData);
        }

        public override void Clear() {
            base.Clear();
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentActionUpdated -= OnCurrentActionUpdated;
            _character.MoveController.MoveRestrictions.RemoveRestriction(nameof(AttackActionState3D));
            _character.MoveController.LookRestrictions.RemoveRestriction(nameof(AttackActionState3D));
        }

        private void PerformAttack(AttackData attackData) {
            // set datas for this attack
            _animationData = attackData.AnimationData;
            SetHitboxInfos(attackData.HitboxDatas);

            // correct the character's intended attack direction
            Vector3 intendedLook = _character.MoveController.Rotation;
            if(intendedLook.magnitude > 0f) {
                _character.MoveController.OverrideRotation(intendedLook);
            }
            PerformActionStartSubActions(attackData.OnActionStartSubActions);

            // add movement restrictions
            _character.MoveController.MoveRestrictions.AddRestriction(nameof(AttackActionState3D));
            _character.MoveController.LookRestrictions.AddRestriction(nameof(AttackActionState3D));
            // update animations
            _character.AnimationController.UpdateAnimationState(_animationData);
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentActionUpdated += OnCurrentActionUpdated;
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos(IReadOnlyList<CombatHitboxDataEntry3D> hitboxDatas) {
            _combatHitBoxData.Clear();
            for (int i = 0; i < hitboxDatas.Count; i++) {
                // add the hitbox by id to reference in case a hit is triggered
                CombatHitBoxData modifierInfo = hitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(hitboxDatas[i].Id, modifierInfo);
                // initialize the hitbox with a combat info
                CombatHitboxInfo3D newInfo = new CombatHitboxInfo3D(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(hitboxDatas[i].Id, newInfo);
            }
        }

        private void PerformActionStartSubActions(IReadOnlyList<SubActionData> subActions) {
            for(int i = 0; i < subActions.Count; i++) {
                SubActionData subAction = subActions[i];
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
            hurtBox.SendHitEvent(_character, hitBox, OnCharacterHit);
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
        private void OnCharacterHit(ICharacter otherCharacter) {
            if(otherCharacter == null) {
                return;
            }
            OnDamageableHit(otherCharacter.Damageable);
        }

        // upon hitting a damageable object
        private void OnDamageableHit(IDamageable damageable) {
            damageable.ReceiveHit(_hitEventInfo);
        }

        private void OnCurrentActionUpdated() {
            // mark this completed if another action has taken over
            if (_character.ActionController.CurrentState != this) {
                UpdateActionStatus(ActionStatus.Completed);
            }
        }

        private void OnAnimationStateUpdated(AnimationState animationState) {
            switch (animationState) {
                case AnimationState.Started:
                    UpdateActionStatus(ActionStatus.Started);
                    break;
                case AnimationState.InProgress:
                    UpdateActionStatus(ActionStatus.InProgress);
                    break;
                case AnimationState.CanTransition:
                    UpdateActionStatus(ActionStatus.CanTransition);
                    break;
                case AnimationState.Completed:
                    UpdateActionStatus(ActionStatus.Completed);
                    break;
            }
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

    [System.Serializable]
    public class AttackData
    {
        [SerializeField] protected AnimationData _animationData;
        [SerializeField] private List<CombatHitboxDataEntry3D> _combatHitBoxDatas = new List<CombatHitboxDataEntry3D>();
        // for stuff like auto-correcting the character for the attack angle
        [SerializeField] private List<SubActionData> _onActionStartSubActions;


        public AnimationData AnimationData => _animationData;
        public IReadOnlyList<CombatHitboxDataEntry3D> HitboxDatas => _combatHitBoxDatas;
        public IReadOnlyList<SubActionData> OnActionStartSubActions => _onActionStartSubActions;
    }
}

