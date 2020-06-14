using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ActionSet {

    }
    
    public interface IPlayerGameplayActionSet {

        CharacterActionData Btn1Skill_Normal { get; }
        CharacterActionData Btn2Skill_Normal { get; }
        CharacterActionData Btn3Skill_Normal { get; }
        CharacterActionData Btn4Skill_Normal { get; }

        CharacterActionData SkillMode1 { get; }
        CharacterActionData SkillMode2 { get; }
    }
    
    public class PlayerGameplayActionSet : IPlayerGameplayActionSet {

        private PlayerCharacter _character;

        private int _currentAttackIndex = 0;
        private List<CharacterActionData> _normalAttacks = new List<CharacterActionData>();
        
        public CharacterActionData Btn1Skill_Normal { get; private set; }
        public CharacterActionData Btn2Skill_Normal => GetNextAttackData();
        public CharacterActionData Btn3Skill_Normal { get; private set; }
        public CharacterActionData Btn4Skill_Normal { get; private set; }

        public CharacterActionData SkillMode1 { get; private set; }
        public CharacterActionData SkillMode2 { get; private set; }

        public PlayerGameplayActionSet(
            PlayerCharacter character,
            CharacterActionData jumpAction,
            List<CharacterActionData> normalAttacks,
            CharacterActionData secondaryAttack,
            CharacterActionData interactAction,
            CharacterActionData skillMode1,
            CharacterActionData skillMode2
            ) {
            _character = character;
            Btn1Skill_Normal = jumpAction;
            _normalAttacks = normalAttacks;
            Btn3Skill_Normal = secondaryAttack;
            Btn4Skill_Normal = interactAction;
            SkillMode1 = skillMode1;
            SkillMode2 = skillMode2;

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
}
