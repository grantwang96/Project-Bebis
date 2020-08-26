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

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            if(!CanPerform(character, state)) {
                return base.Initiate(character, state, context);
            }
            BasicTriggerAttackState triggerActionState = new BasicTriggerAttackState(this, character);
            CharacterActionResponse response = new CharacterActionResponse(true, Bufferable, triggerActionState);
            return response;
        }

        private bool CanPerform(ICharacter character, ICharacterActionState state) {
            return state == null ||
                state.Status.HasFlag(ActionStatus.CanTransition) ||
                state.Status.HasFlag(ActionStatus.Completed);
        }
    }

    public class BasicTriggerAttackState : TriggerActionState {

        private BasicTriggerAttackActionData _data;

        public BasicTriggerAttackState(BasicTriggerAttackActionData data, ICharacter character) : base(data, character) {
            _data = data;
            PerformActionStartSubAction();
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
            _target.Damageable.TakeDamage(hitEventInfo);
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
