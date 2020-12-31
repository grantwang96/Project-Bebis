using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Normal Attack")]
    public class NormalAttackActionData : CharacterActionDataV2
    {
        [SerializeField] private List<AttackData> _attackData = new List<AttackData>();
        [SerializeField] private CharacterActionTag _allowedTransitionMoves;
        public IReadOnlyList<AttackData> AttackDatas => _attackData;
        public CharacterActionTag AllowedTransitionMoves => _allowedTransitionMoves;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new NormalAttackActionState(this, character);
        }

        protected override bool CanPerformAction(ICharacterV2 character, ICharacterActionStateV2 foundActionState, CharacterActionContext context) {
            ICharacterActionStateV2 currentAction = character.ActionController.CurrentState;
            bool canPerform = currentAction?.CanInterrupt(context, this) ?? true;
            canPerform &= foundActionState?.CanPerform(context, this) ?? true;
            return canPerform;
        }

        protected override ICharacterActionStateV2 HandleActionSuccess(ICharacterV2 character, ICharacterActionStateV2 foundActionState) {
            if (foundActionState != null && foundActionState.Data == this) {
                return foundActionState;
            }
            foundActionState?.Clear();
            return CreateActionState(character);
        }
    }

    public class NormalAttackActionState : CharacterActionStateV2
    {
        private NormalAttackActionData _data;
        private readonly Dictionary<string, CombatHitBoxData> _combatHitBoxData = new Dictionary<string, CombatHitBoxData>();
        private int _currentAttackIndex = 0;
        private HitEventInfoV2 _hitEventInfo;

        private readonly List<IDamageableV2> _hitDamageables = new List<IDamageableV2>();

        public NormalAttackActionState(NormalAttackActionData data, ICharacterV2 character) : base(data, character) {
            _data = data;
            Status = ActionStatus.Started;
            // add movement restrictions
            _character.MoveController.MovementRestrictions.AddRestriction(nameof(AttackActionStateV2));
            _character.MoveController.LookRestrictions.AddRestriction(nameof(AttackActionStateV2));
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated += OnCurrentActionUpdated;
        }

        public override bool CanInterrupt(CharacterActionContext context, ICharacterActionDataV2 data) {
            bool canInterrupt = (_data.AllowedTransitionMoves & data.Tags) != 0;
            return canInterrupt;
        }

        public override bool CanPerform(CharacterActionContext context, ICharacterActionDataV2 data) {
            bool canPerform = Status == ActionStatus.CanTransition;
            canPerform &= (_data.ActionableContexts & context) != 0;
            return canPerform;
        }

        public override void Initiate() {
            base.Initiate();
            if(_currentAttackIndex >= _data.AttackDatas.Count) {
                return;
            }
            PerformAttack(_data.AttackDatas[_currentAttackIndex]);
            _currentAttackIndex++;
        }
        public override void Clear() {
            base.Clear();
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
            _character.ActionController.OnCurrentStateUpdated -= OnCurrentActionUpdated;
            _character.MoveController.MovementRestrictions.RemoveRestriction(nameof(AttackActionStateV2));
            _character.MoveController.LookRestrictions.RemoveRestriction(nameof(AttackActionStateV2));
        }

        private void PerformAttack(AttackData attackData) {
            _hitDamageables.Clear();
            // set datas for this attack
            SetHitboxInfos(attackData.HitboxDatas);

            // correct the character's intended attack direction
            Vector3 intendedLook = _character.GameObject.transform.forward;
            if (intendedLook.magnitude > 0f) {
                _character.MoveController.OverrideRotation(intendedLook);
            }
            PerformActionStartSubActions(attackData.OnActionStartSubActionsV2);
            // update animations
            _character.AnimationController.UpdateAnimationState(attackData.AnimationData);
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
            }
        }

        private void OnAnimationStateUpdated(AnimationState animationState) {
            switch (animationState) {
                case AnimationState.Started:
                    UpdateActionStatus(ActionStatus.Started);
                    break;
                case AnimationState.InProgress:
                    UpdateActionStatus(ActionStatus.InProgress);
                    OnActionInProgress();
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
