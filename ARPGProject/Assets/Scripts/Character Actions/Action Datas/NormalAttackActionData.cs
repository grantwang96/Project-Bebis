using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    [CreateAssetMenu(menuName = "Character Actions V2/Normal Attack")]
    public class NormalAttackActionData : BaseAttackActionData
    {
        [SerializeField] private List<AttackData> _attackData = new List<AttackData>();
        public IReadOnlyList<AttackData> AttackDatas => _attackData;

        protected override ICharacterActionStateV2 CreateActionState(ICharacterV2 character) {
            return new NormalAttackActionState(this, character);
        }

        protected override ICharacterActionStateV2 HandleActionSuccess(ICharacterV2 character, ICharacterActionStateV2 foundActionState) {
            if (foundActionState != null && foundActionState.Data == this) {
                return foundActionState;
            }
            foundActionState?.Interrupt();
            return CreateActionState(character);
        }
    }

    public class NormalAttackActionState : BaseAttackActionState
    {
        private NormalAttackActionData _data;
        private int _currentAttackIndex = 0;

        public NormalAttackActionState(NormalAttackActionData data, ICharacterV2 character) : base(data, character) {
            _data = data;
        }

        public override void Initiate() {
            base.Initiate();
            if(_currentAttackIndex >= _data.AttackDatas.Count) {
                return;
            }
            PerformAttack(_data.AttackDatas[_currentAttackIndex]);
            _currentAttackIndex++;
        }
    }
}
