using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Attack")]
    public class AttackActionDataV2 : BaseAttackActionData
    {
        [SerializeField] private AttackData _attackData;
        public AttackData AttackData => _attackData;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new AttackActionStateV2(this, character);
        }
    }

    public class AttackActionStateV2 : BaseAttackActionState
    {
        private AttackActionDataV2 _data;

        public AttackActionStateV2(AttackActionDataV2 data, ICharacterV2 character) : base(data, character) {
            // initialize data
            _data = data;
        }

        protected override void OnPerformAction(CharacterActionContext context) {
            base.OnPerformAction(context);
            UnsetRestrictions();
            UnsubscribeToEvents();
            SubscribeToEvents();
            SetRestrictions();
            PerformAttack(_data.AttackData);
        }
    }
}
