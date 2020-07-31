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

        CharacterActionData SkillMode1 { get; }
        CharacterActionData SkillMode2 { get; }

        void Override(IPlayerSkillsLoadout skillsLoadout);
    }

    public class PlayerNormalGameplayActionSet : IPlayerGameplayActionSet {

        private PlayerCharacter _player;

        private int _currentAttackIndex = 0;
        private List<CharacterActionData> _groundedNormalAttacks = new List<CharacterActionData>();
        private CharacterActionData _normalAerialAttack;
        private CharacterActionData _secondaryAttack;
        private CharacterActionData _secondaryAerialAttack;

        public CharacterActionData Btn1Skill { get; private set; }
        public CharacterActionData Btn2Skill => GetNormalAttackData();
        public CharacterActionData Btn3Skill => GetSecondaryAttackData();
        public CharacterActionData Btn4Skill { get; private set; }

        public CharacterActionData SkillMode1 { get; private set; }
        public CharacterActionData SkillMode2 { get; private set; }

        // only utilize the "normal" skills in this action set
        public PlayerNormalGameplayActionSet(PlayerCharacter player, IPlayerSkillsLoadout playerSkillsLoadout) {
            _player = player;
            _player.ActionController.OnActionStatusUpdated += OnActionStatusUpdated;
            Override(playerSkillsLoadout);
        }

        public void Override(IPlayerSkillsLoadout playerSkillsLoadout) {
            Btn1Skill = playerSkillsLoadout.NormalBtn1Skill;
            _groundedNormalAttacks = new List<CharacterActionData>(playerSkillsLoadout.NormalAttackSkills);
            _normalAerialAttack = playerSkillsLoadout.NormalAerialAttackSkill;
            _secondaryAttack = playerSkillsLoadout.SecondaryAttackSkill;
            _secondaryAerialAttack = playerSkillsLoadout.SecondaryAerialAttackSkill;
            Btn4Skill = playerSkillsLoadout.NormalBtn4Skill;

            SkillMode1 = playerSkillsLoadout.RightTriggerSkill;
            SkillMode2 = playerSkillsLoadout.LeftTriggerSkill;
        }

        private void OnActionStatusUpdated(ActionStatus status) {
            if (status == ActionStatus.Started && _player.ActionController.CurrentState.Data == _groundedNormalAttacks[_currentAttackIndex]) {
                IncrementAttackIndex();
            }
        }

        private CharacterActionData GetNormalAttackData() {
            return _player.MoveController.IsGrounded ? GetNextGroundedAttackData() : GetNormalAerialAttackData();
        }

        private CharacterActionData GetNextGroundedAttackData() {
            if (_player.ActionController.CurrentState == null) {
                _currentAttackIndex = 0;
                if (_groundedNormalAttacks.Count == 0) {
                    return null;
                }
                return _groundedNormalAttacks[0];
            }
            ActionStatus status = _player.ActionController.CurrentState.Status;
            if (status.HasFlag(ActionStatus.CanBufferInput) || status.HasFlag(ActionStatus.CanTransition) || status.HasFlag(ActionStatus.Completed)) {
                return _groundedNormalAttacks[_currentAttackIndex];
            }
            return null;
        }

        private CharacterActionData GetNormalAerialAttackData() {
            _currentAttackIndex = 0;
            return _normalAerialAttack;
        }

        private void IncrementAttackIndex() {
            _currentAttackIndex++;
            if(_currentAttackIndex >= _groundedNormalAttacks.Count) {
                _currentAttackIndex = 0;
            }
        }

        private CharacterActionData GetSecondaryAttackData() {
            return _player.MoveController.IsGrounded ? _secondaryAttack : _secondaryAerialAttack;
        }
    }

    public class PlayerSkillsActionSet : IPlayerGameplayActionSet {
        public CharacterActionData Btn1Skill { get; private set; }
        public CharacterActionData Btn2Skill { get; private set; }
        public CharacterActionData Btn3Skill { get; private set; }
        public CharacterActionData Btn4Skill { get; private set; }

        public CharacterActionData SkillMode1 { get; private set; }
        public CharacterActionData SkillMode2 { get; private set; }

        private bool _skillSet1;

        public PlayerSkillsActionSet(bool skillSet1, IPlayerSkillsLoadout skillsLoadout) {
            _skillSet1 = skillSet1;
        }

        public void Override(IPlayerSkillsLoadout skillsLoadout) {
            if (_skillSet1) {
                Btn1Skill = skillsLoadout.SkillSet1Btn1Skill;
                Btn2Skill = skillsLoadout.SkillSet1Btn2Skill;
                Btn3Skill = skillsLoadout.SkillSet1Btn3Skill;
                Btn4Skill = skillsLoadout.SkillSet1Btn4Skill;
            } else {
                Btn1Skill = skillsLoadout.SkillSet2Btn1Skill;
                Btn2Skill = skillsLoadout.SkillSet2Btn2Skill;
                Btn3Skill = skillsLoadout.SkillSet2Btn3Skill;
                Btn4Skill = skillsLoadout.SkillSet2Btn4Skill;
            }
            SkillMode1 = skillsLoadout.RightTriggerSkill;
            SkillMode2 = skillsLoadout.LeftTriggerSkill;
        }
    }
}
