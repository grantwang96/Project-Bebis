using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ActionSet {

    }
    
    public interface IPlayerGameplayActionSet {

        CharacterActionData A_BtnSkill { get; }
        CharacterActionData B_BtnSkill { get; }
        CharacterActionData X_BtnSkill { get; }
        CharacterActionData Y_BtnSkill { get; }
    }
    
    public class PlayerGameplayActionSet_NormalMode : IPlayerGameplayActionSet {

        private PlayerCharacter _character;

        private int _currentAttackIndex = 0;
        private List<AttackActionData> _normalAttacks = new List<AttackActionData>();
        private AttackActionData _secondaryAttack;

        public CharacterActionData A_BtnSkill => GetNextAttackData();
        public CharacterActionData B_BtnSkill { get; private set; }
        public CharacterActionData X_BtnSkill { get; private set; }
        public CharacterActionData Y_BtnSkill { get; private set; }

        public PlayerGameplayActionSet_NormalMode(
            PlayerCharacter character,
            List<AttackActionData> normalAttacks,
            AttackActionData secondaryAttack
            ) {
            _character = character;
            _normalAttacks = normalAttacks;
            _secondaryAttack = secondaryAttack;

            _character.ActionController.OnPerformActionSuccess += OnPerformActionSuccess;
        }

        private void OnPerformActionSuccess() {
            if(_character.ActionController.CurrentState.Data == _normalAttacks[_currentAttackIndex]) {
                IncrementAttackIndex();
            }
        }

        private AttackActionData GetNextAttackData() {
            if(_character.ActionController.CurrentState == null) {
                _currentAttackIndex = 0;
                return _normalAttacks[0];
            }
            ActionStatus status = _character.ActionController.CurrentState.Status;
            if(status.HasFlag(ActionStatus.CanTransition) || status.HasFlag(ActionStatus.Completed)) {
                return _normalAttacks[_currentAttackIndex];
            }
            return null;
        }

        private void IncrementAttackIndex() {
            _currentAttackIndex++;
            if(_currentAttackIndex >= _normalAttacks.Count) {
                _currentAttackIndex = 0;
            }
        }
    }

    public class PlayerGameplayActionSet_SkillMode : IPlayerGameplayActionSet {

        private PlayerCharacter _character;

        public CharacterActionData A_BtnSkill { get; private set; }
        public CharacterActionData B_BtnSkill { get; private set; }
        public CharacterActionData X_BtnSkill { get; private set; }
        public CharacterActionData Y_BtnSkill { get; private set; }
    }
}
