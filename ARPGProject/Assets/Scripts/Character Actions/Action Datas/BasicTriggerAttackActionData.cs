using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/3D/BasicTriggerAttack")]
    public class BasicTriggerAttackActionData : TriggerActionData {

        [SerializeField] private CombatHitBoxData _onHitData;
        [SerializeField] private List<SubActionData> _onActionStartSubActions = new List<SubActionData>();
        public CombatHitBoxData OnHitData => _onHitData;
        public IReadOnlyList<SubActionData> OnActionStartSubActions => _onActionStartSubActions;

        public override CharacterActionResponse Hold(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            return FailedActionResponse(character);
        }

        public override CharacterActionResponse Release(ICharacter character, ICharacterActionState foundActionState, CharacterActionContext context) {
            return FailedActionResponse(character);
        }

        private bool CanPerform(ICharacter character, ICharacterActionState state) {
            ICharacterActionState currentState = character.ActionController.CurrentState;
            return currentState == null ||
                currentState.Status.HasFlag(ActionStatus.CanTransition) ||
                currentState.Status.HasFlag(ActionStatus.Completed); 
        }

        protected override bool CanPerformAction(ICharacter character, ICharacterActionState foundActionState) {
            return CanPerform(character, foundActionState);
        }

        protected override ICharacterActionState CreateActionState(ICharacter character) {
            return new BasicTriggerAttackState(this, character);
        }
    }

    public class BasicTriggerAttackState : TriggerActionState {

        private BasicTriggerAttackActionData _data;

        public BasicTriggerAttackState(BasicTriggerAttackActionData data, ICharacter character) : base(data, character) {
            _data = data;
        }

        public override void Initiate() {
            base.Initiate();
            PerformActionStartSubAction();
            PerformTriggerAction();
        }

        private void PerformActionStartSubAction() {
            for (int i = 0; i < _data.OnActionStartSubActions.Count; i++) {
                SubActionData subAction = _data.OnActionStartSubActions[i];
                subAction.PerformAction(subAction.CreateInitData(_character));
            }
        }

        protected override void OnTriggerEffect() {
            CombatHitBoxData hitBoxData = _data.OnHitData;
            HitEventInfo hitEventInfo = new HitEventInfo() {
                Power = CalculatePower(_character.CharacterStatManager, hitBoxData.BasePower, hitBoxData.PowerRange),
                KnockBackDirection = CalculateRelativeDirection(_character.MoveController.Body, hitBoxData.KnockbackAngle),
                Force = hitBoxData.KnockbackForce,
                OverrideForce = hitBoxData.OverrideForce,
                Attacker = _character
            };
            _target.Damageable.ReceiveHit(hitEventInfo);
        }

        protected override void PerformTriggerAction() {
            base.PerformTriggerAction();
            _character.MoveController.MoveRestrictions.AddRestriction(_data.ActionName);
            _character.MoveController.LookRestrictions.AddRestriction(_data.ActionName);
        }

        public override void Clear() {
            base.Clear();
            _character.MoveController.MoveRestrictions.RemoveRestriction(_data.ActionName);
            _character.MoveController.LookRestrictions.RemoveRestriction(_data.ActionName);
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
    }
}
