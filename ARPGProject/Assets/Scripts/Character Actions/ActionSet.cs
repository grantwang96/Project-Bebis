using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public interface IPlayerGameplayActionSet {

        CharacterActionData Btn1Skill { get; }
        CharacterActionData Btn2Skill { get; }
        CharacterActionData Btn3Skill { get; }
        CharacterActionData Btn4Skill { get; }

        CharacterActionData SkillMode1 { get; }
        CharacterActionData SkillMode2 { get; }

        void Override(IPlayerSkillsLoadout skillsLoadout);
    }

    public interface IPlayerActionSet
    {
        string ButtonSouthAction { get; }
        string ButtonEastAction { get; }
        string ButtonNorthAction { get; }
        string ButtonWestAction { get; }

        void Update(IPlayerSkillsLoadoutV2 loadout, ICharacterV2 playerCharacter);
    }

    public class PlayerNormalActionSet : IPlayerActionSet
    {
        public string ButtonSouthAction { get; private set; }
        public string ButtonEastAction { get; private set; }
        public string ButtonNorthAction => GetSecondaryAttack();
        public string ButtonWestAction => GetNextNormalAttack();

        private readonly List<string> _groundedNormalAttacks = new List<string>();
        private int _currentGroundedNormalAttackIndex = 0;
        private string _normalAerialAttack;
        private string _secondaryAttack;
        private string _secondaryAerialAttack;
        private ICharacterV2 _playerCharacter;

        public PlayerNormalActionSet(IPlayerSkillsLoadoutV2 loadout, ICharacterV2 playerCharacter) {
            Update(loadout, playerCharacter);
        }

        public void Update(IPlayerSkillsLoadoutV2 loadout, ICharacterV2 playerCharacter) {
            _playerCharacter = playerCharacter;
            _playerCharacter.ActionController.OnActionStateUpdated += OnActionStateUpdated;
            _currentGroundedNormalAttackIndex = 0;
            ButtonSouthAction = loadout.NormalBtn1Skill?.Id;
            ButtonEastAction = loadout.NormalBtn4Skill?.Id;
            _secondaryAttack = loadout.SecondaryAttackSkill?.Id;
        }

        private string GetNextNormalAttack() {
            if(_currentGroundedNormalAttackIndex > _groundedNormalAttacks.Count) {
                return "";
            }
            return _groundedNormalAttacks[_currentGroundedNormalAttackIndex];
        }

        private string GetSecondaryAttack() {
            return _secondaryAttack;
        }

        private void OnActionStateUpdated(ICharacterActionStateV2 actionState) {
            if (actionState.Data.Id.Equals(_groundedNormalAttacks[_currentGroundedNormalAttackIndex])) {
                if (actionState.Status == ActionStatus.Completed) {
                    _currentGroundedNormalAttackIndex = 0;
                }
                if (actionState.Status == ActionStatus.Started) {
                    _currentGroundedNormalAttackIndex++;
                    if (_currentGroundedNormalAttackIndex >= _groundedNormalAttacks.Count) {
                        _currentGroundedNormalAttackIndex = 0;
                    }
                }
            }
        }
    }

    public class PlayerSkillsActionSetV2 : IPlayerActionSet
    {
        public string ButtonSouthAction { get; private set; }
        public string ButtonEastAction { get; private set; }
        public string ButtonNorthAction { get; private set; }
        public string ButtonWestAction { get; private set; }

        private ICharacterV2 _playerCharacter;
        private bool _skillSet1;

        public PlayerSkillsActionSetV2(IPlayerSkillsLoadoutV2 loadout, ICharacterV2 playerCharacter, bool skillSet1) {
            _skillSet1 = skillSet1;
            Update(loadout, playerCharacter);
        }

        public void Update(IPlayerSkillsLoadoutV2 loadout, ICharacterV2 playerCharacter) {
            if (_skillSet1) {
                ButtonSouthAction = loadout.SkillSet1Btn1Skill?.Id;
                ButtonEastAction = loadout.SkillSet1Btn4Skill?.Id;
                ButtonNorthAction = loadout.SkillSet1Btn3Skill?.Id;
                ButtonWestAction = loadout.SkillSet1Btn2Skill?.Id;
            } else {
                ButtonSouthAction = loadout.SkillSet2Btn1Skill?.Id;
                ButtonEastAction = loadout.SkillSet2Btn4Skill?.Id;
                ButtonNorthAction = loadout.SkillSet2Btn3Skill?.Id;
                ButtonWestAction = loadout.SkillSet2Btn2Skill?.Id;
            }
        }
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
            _player.ActionController.OnCurrentActionUpdated += OnCurrentActionUpdated;
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

        private void OnCurrentActionUpdated() {
            if(_player.ActionController.CurrentState == null) {
                _currentAttackIndex = 0;
            } else if(_player.ActionController.CurrentState.Data == _groundedNormalAttacks[_currentAttackIndex]) {
                _player.ActionController.CurrentState.OnActionStatusUpdated += OnActionStatusUpdated;
            }
        }

        private void OnActionStatusUpdated(ICharacterActionState state, ActionStatus status) {
            if (_player.ActionController.CurrentState != null
                && _player.ActionController.CurrentState.Data == _groundedNormalAttacks[_currentAttackIndex]) {
                switch (status) {
                    case ActionStatus.Started:
                        IncrementAttackIndex();
                        break;
                    case ActionStatus.Completed:
                        state.OnActionStatusUpdated -= OnActionStatusUpdated;
                        break;
                }
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
            Override(skillsLoadout);
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
