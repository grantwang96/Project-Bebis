using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public abstract class BaseAttackActionData : CharacterActionDataV2
    {
        protected override ICharacterActionStateV2 HandleActionSuccess(ICharacterV2 character, ICharacterActionStateV2 foundActionState) {
            foundActionState?.Interrupt();
            return CreateActionState(character);
        }
    }

    public abstract class BaseAttackActionState : CharacterActionStateV2
    {
        protected readonly Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();
        protected readonly List<IDamageableV2> _hitDamageables = new List<IDamageableV2>();
        protected HitEventInfoV2 _hitEventInfo;

        public BaseAttackActionState(BaseAttackActionData data, ICharacterV2 character) : base(data, character) {
            // add listeners
            _character.MoveController.OnIsGroundedUpdated += OnIsGroundedUpdated;
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated += OnCurrentActionUpdated;
            // add movement restrictions
            _character.MoveController.MovementRestrictions.AddRestriction(Data.Id);
            _character.MoveController.LookRestrictions.AddRestriction(Data.Id);
        }

        public override void Clear() {
            base.Clear();
            // remove references to hitboxes
            ClearHitboxInfos();
            // remove listeners
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated -= OnCurrentActionUpdated;
            _character.MoveController.OnIsGroundedUpdated -= OnIsGroundedUpdated;
            // remove movement restrictions
            _character.MoveController.MovementRestrictions.RemoveRestriction(Data.Id);
            _character.MoveController.LookRestrictions.RemoveRestriction(Data.Id);
        }

        protected void PerformAttack(AttackData attackData) {
            // set hitbox datas for this attack
            SetHitboxInfos(attackData.HitboxDatas);

            // correct the character's intended attack direction
            Vector3 intendedLook = _character.UnitController.RotationInput;
            if (intendedLook.magnitude > 0f) {
                _character.MoveController.OverrideRotation(intendedLook);
            }
            PerformActionStartSubActions(attackData.OnActionStartSubActionsV2);
            // update animations
            _character.AnimationController.UpdateAnimationState(attackData.AnimationData);
        }

        // update the hitboxes on the weapon
        protected void SetHitboxInfos(IReadOnlyList<CombatHitboxDataEntry> hitboxDatas) {
            ClearHitboxInfos();
            for (int i = 0; i < hitboxDatas.Count; i++) {
                // add the hitbox by id to reference in case a hit is triggered
                CombatHitBoxData modifierInfo = hitboxDatas[i].CombatHitboxData;
                _combatHitBoxData.Add(hitboxDatas[i].Id, modifierInfo);
                // initialize the hitbox with a combat info
                HitboxInfoV2 newInfo = new HitboxInfoV2(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(hitboxDatas[i].Id, newInfo);
            }
        }

        protected void ClearHitboxInfos() {
            foreach (string key in _combatHitBoxData.Keys) {
                _character.HitboxController.Clear(key);
            }
            // Debug.Log("Clearing hit box data");
            _combatHitBoxData.Clear();
        }

        protected void PerformActionStartSubActions(IReadOnlyList<SubActionDataV2> subActions) {
            for (int i = 0; i < subActions.Count; i++) {
                SubActionDataV2 subAction = subActions[i];
                subAction.PerformAction(subAction.CreateInitData(_character));
            }
        }

        protected void OnHitboxTriggered(HitboxV2 hitBox, Collider collider) {
            // if failed to retrieve hitbox data
            if (!_combatHitBoxData.TryGetValue(hitBox.name, out CombatHitBoxData hitBoxData)) {
                CustomLogger.Warn(nameof(NormalAttackActionState), $"Could not retrieve hitbox data for hitbox {hitBox.name}");
                return;
            }
            // find out what type of thing we hit
            HurtboxV2 hurtBox = collider.GetComponent<HurtboxV2>();
            // if the thing we hit was not a hurtbox
            if (hurtBox == null) {
                IDamageableV2 damageable = collider.GetComponent<IDamageableV2>();
                if (damageable != null && damageable != _character.Damageable && !_hitDamageables.Contains(damageable)) {
                    // create and set the hit event info
                    SetHitEventInfo(hitBoxData, hitBox);
                    OnDamageableHit(damageable);
                }
                Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    rigidbody.AddForce(_hitEventInfo.KnockBackDirection * _hitEventInfo.Force, ForceMode.Impulse);
                }
                return;
            }
            // ensure this isn't the character's own hitbox or a character we've already hit
            if (hurtBox.Character == _character || _hitDamageables.Contains(hurtBox.Character.Damageable)) {
                return;
            }
            // create and set the hit event info
            SetHitEventInfo(hitBoxData, hitBox);
            _hitEventInfo.Hurtbox = hurtBox;
            hurtBox.SendHitEvent(_character, hitBox, OnCharacterHit);
        }

        // sets the hit event info for this attack state
        protected void SetHitEventInfo(CombatHitBoxData hitBoxData, HitboxV2 hitBox) {
            int power = CalculatePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange);
            Vector3 direction = CalculateRelativeDirection(_character.Center, hitBoxData.KnockbackAngle);
            _hitEventInfo = new HitEventInfoV2() {
                Power = power,
                KnockBackDirection = direction,
                Force = hitBoxData.KnockbackForce,
                OverrideForce = hitBoxData.OverrideForce,
                Attacker = _character,
                Hitbox = hitBox
            };
        }

        // upon hitting a character
        protected void OnCharacterHit(ICharacterV2 otherCharacter) {
            if (otherCharacter == null) {
                return;
            }
            OnDamageableHit(otherCharacter.Damageable);
        }

        // upon hitting a damageable object
        protected void OnDamageableHit(IDamageableV2 damageable) {
            if (_hitDamageables.Contains(damageable)) {
                return;
            }
            damageable.ReceiveHit(_hitEventInfo);
            _hitDamageables.Add(damageable);
        }

        protected void OnCurrentActionUpdated(ICharacterActionStateV2 state) {
            // mark this completed if another action has taken over
            if (_character.ActionController.CurrentState != this) {
                UpdateActionStatus(ActionStatus.Completed);
                Clear();
            }
        }

        protected void OnAnimationStateUpdated(AnimationState animationState) {
            switch (animationState) {
                case AnimationState.Started:
                    UpdateActionStatus(ActionStatus.Started);
                    break;
                case AnimationState.InProgress:
                    OnActionInProgress();
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

        protected void OnActionInProgress() {
            // reset the attack state (this is a multi-hit)
            _hitDamageables.Clear();
        }

        protected void OnIsGroundedUpdated(bool isGrounded) {
            // if the character's grounded state changes, cancel this attack action
            UpdateActionStatus(ActionStatus.Completed);
        }

        // calculate total damage
        protected int CalculatePower(ICharacterStatManager characterStatManager, int basePower, MinMax_Int range) {
            int attackPower = characterStatManager.Attack;
            attackPower += basePower;
            attackPower += Random.Range(range.Min, range.Max);
            return attackPower;
        }

        // calculate knockback direction in worldspace
        protected Vector3 CalculateRelativeDirection(Transform transform, Vector3 angle) {
            return transform.TransformDirection(angle);
        }

        // Calculate the general direction that the player will move
        protected Vector3 GetRelativeDirection(Vector3 angle) {
            return _character.Center.TransformDirection(angle);
        }
    }
}
