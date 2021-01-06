using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Attack")]
    public class AttackActionDataV2 : CharacterActionDataV2
    {
        [SerializeField] private AttackData _attackData;
        public AttackData AttackData => _attackData;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new AttackActionStateV2(this, character);
        }

        protected override ICharacterActionStateV2 HandleActionSuccess(ICharacterV2 character, ICharacterActionStateV2 foundActionState) {
            foundActionState?.Interrupt();
            return CreateActionState(character);
        }
    }

    public class AttackActionStateV2 : CharacterActionStateV2
    {
        private AttackActionDataV2 _data;
        private readonly Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();
        private AnimationData _animationData;
        private readonly List<IDamageableV2> _hitDamageables = new List<IDamageableV2>();
        private HitEventInfoV2 _hitEventInfo;

        public AttackActionStateV2(AttackActionDataV2 data, ICharacterV2 character) : base(data, character) {
            // initialize data
            _data = data;
            // add listeners
            _character.MoveController.OnIsGroundedUpdated += OnIsGroundedUpdated;
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated += OnCurrentActionUpdated;
            // add movement restrictions
            _character.MoveController.MovementRestrictions.AddRestriction(_data.Id);
            _character.MoveController.LookRestrictions.AddRestriction(_data.Id);
        }

        public override void Initiate() {
            base.Initiate();
            PerformAttack(_data.AttackData);
            UpdateActionStatus(ActionStatus.Started);
        }

        public override void Clear() {
            base.Clear();
            // remove listeners
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated -= OnCurrentActionUpdated;
            _character.MoveController.OnIsGroundedUpdated -= OnIsGroundedUpdated;
            // remove movement restrictions
            _character.MoveController.MovementRestrictions.RemoveRestriction(_data.Id);
            _character.MoveController.LookRestrictions.RemoveRestriction(_data.Id);
        }

        private void PerformAttack(AttackData attackData) {
            // set datas for this attack
            _animationData = attackData.AnimationData;
            SetHitboxInfos(attackData.HitboxDatas);

            // correct the character's intended attack direction
            Vector3 intendedLook = _character.UnitController.RotationInput;
            if (intendedLook.magnitude > 0f) {
                _character.MoveController.OverrideRotation(intendedLook);
            }
            PerformActionStartSubActions(attackData.OnActionStartSubActionsV2);
            // update animations
            _character.AnimationController.UpdateAnimationState(_animationData);
        }

        // update the hitboxes on the weapon
        private void SetHitboxInfos(IReadOnlyList<CombatHitboxDataEntry> hitboxDatas) {
            _combatHitBoxData.Clear();
            for (int i = 0; i < hitboxDatas.Count; i++) {
                // add the hitbox by id to reference in case a hit is triggered
                CombatHitBoxData modifierInfo = hitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(hitboxDatas[i].Id, modifierInfo);
                // initialize the hitbox with a combat info
                HitboxInfoV2 newInfo = new HitboxInfoV2(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(hitboxDatas[i].Id, newInfo);
            }
        }

        private void PerformActionStartSubActions(IReadOnlyList<SubActionDataV2> subActions) {
            for (int i = 0; i < subActions.Count; i++) {
                SubActionDataV2 subAction = subActions[i];
                subAction.PerformAction(subAction.CreateInitData(_character));
            }
        }

        private void OnHitboxTriggered(HitboxV2 hitBox, Collider collider) {
            // if failed to retrieve hitbox data
            if (!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData3D), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            SetHitEventInfo(hitBoxData);
            // create and set the hit event info
            // find out what type of thing we hit
            HurtboxV2 hurtBox = collider.GetComponent<HurtboxV2>();
            // if the thing we hit was not a hurtbox
            if (hurtBox == null) {
                IDamageableV2 damageable = collider.GetComponent<IDamageableV2>();
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
            if (hurtBox.Character == _character) {
                return;
            }
            hurtBox.SendHitEvent(_character, hitBox, OnCharacterHit);
        }

        // sets the hit event info for this attack state
        private void SetHitEventInfo(CombatHitBoxData hitBoxData) {
            int power = CalculatePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(_character.Center, hitBoxData.KnockbackAngle);
            _hitEventInfo = new HitEventInfoV2() {
                Power = power,
                KnockBackDirection = direction,
                Force = hitBoxData.KnockbackForce,
                OverrideForce = hitBoxData.OverrideForce,
                Attacker = _character
            };
        }

        // upon hitting a character
        private void OnCharacterHit(ICharacterV2 otherCharacter) {
            if (otherCharacter == null) {
                return;
            }
            OnDamageableHit(otherCharacter.Damageable);
        }

        // upon hitting a damageable object
        private void OnDamageableHit(IDamageableV2 damageable) {
            if (_hitDamageables.Contains(damageable)) {
                return;
            }
            damageable.ReceiveHit(_hitEventInfo);
            _hitDamageables.Add(damageable);
        }

        private void OnCurrentActionUpdated(ICharacterActionStateV2 state) {
            // mark this completed if another action has taken over
            if (_character.ActionController.CurrentState != this) {
                UpdateActionStatus(ActionStatus.Completed);
                Clear();
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

        private void OnActionInProgress() {
            // reset the attack state (this is a multi-hit)
            _hitDamageables.Clear();
        }

        private void OnIsGroundedUpdated(bool isGrounded) {
            // if the character's grounded state changes, cancel this attack action
            UpdateActionStatus(ActionStatus.Completed);
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
            return _character.Center.TransformDirection(angle);
        }
    }
}
