using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ActionSet {

    }
    
    public interface IPlayerGameplayActionSet {

        CharacterActionData Btn1Skill { get; }
        CharacterActionData Btn2Skill { get; }
        CharacterActionData Btn3Skill { get; }
        CharacterActionData Btn4Skill { get; }
    }
    
    public class PlayerGameplayActionSet_NormalMode : IPlayerGameplayActionSet {

        private PlayerCharacter _character;

        private int _currentAttackIndex = 0;
        private List<CharacterActionData> _normalAttacks = new List<CharacterActionData>();
        
        public CharacterActionData Btn1Skill { get; private set; }
        public CharacterActionData Btn2Skill => GetNextAttackData();
        public CharacterActionData Btn3Skill { get; private set; }
        public CharacterActionData Btn4Skill { get; private set; }

        public PlayerGameplayActionSet_NormalMode(
            PlayerCharacter character,
            CharacterActionData jumpAction,
            List<CharacterActionData> normalAttacks,
            CharacterActionData secondaryAttack,
            CharacterActionData interactAction
            ) {
            _character = character;
            Btn1Skill = jumpAction;
            _normalAttacks = normalAttacks;
            Btn3Skill = secondaryAttack;
            Btn4Skill = interactAction;

            _character.ActionController.OnActionStatusUpdated += OnActionStatusUpdated;
        }

        private void OnActionStatusUpdated(ActionStatus status) {
            if(status == ActionStatus.Started && _character.ActionController.CurrentState.Data == _normalAttacks[_currentAttackIndex]) {
                IncrementAttackIndex();
            }
        }

        private CharacterActionData GetNextAttackData() {
            if(_character.ActionController.CurrentState == null) {
                _currentAttackIndex = 0;
                if (_normalAttacks.Count == 0) {
                    return null;
                }
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

        public CharacterActionData Btn1Skill { get; private set; }
        public CharacterActionData Btn2Skill { get; private set; }
        public CharacterActionData Btn3Skill { get; private set; }
        public CharacterActionData Btn4Skill { get; private set; }
    }
}
