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
    }

    public class NormalAttackActionState : BaseAttackActionState
    {
        private NormalAttackActionData _data;
        private int _currentAttackIndex = 0;

        public NormalAttackActionState(NormalAttackActionData data, ICharacterV2 character) : base(data, character) {
            _data = data;
        }

        protected override void OnPerformAction(CharacterActionContext context) {
            base.OnPerformAction(context);
            if(_character.ActionController.CurrentState != this) {
                _character.ActionController.CurrentState?.Interrupt();
            }
            PerformNormalAttack();
        }

        private void PerformNormalAttack() {
            if (_currentAttackIndex >= _data.AttackDatas.Count) {
                Debug.LogError($"[{Data.Id}]: Exceeded attack string data!");
                return;
            }
            // clean up previous attack
            UnsubscribeToEvents();
            UnsetRestrictions();
            // listen for events and add restrictions
            SubscribeToEvents();
            SetRestrictions();
            // perform the attack
            PerformAttack(_data.AttackDatas[_currentAttackIndex]);
            // increment the attack index
            _currentAttackIndex++;
        }

        protected override void OnFinishedAttack() {
            base.OnFinishedAttack();
            _currentAttackIndex = 0;
        }

        protected override void OnAnimationCompleted() {
            // ignore if this action has only just started (in case of leftover animation messages from previous states)
            if (Status == ActionStatus.Ready || Status == ActionStatus.Started) {
                return;
            }
            base.OnAnimationCompleted();
        }
    }
}
