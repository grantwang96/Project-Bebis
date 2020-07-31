using UnityEngine;
using System.Collections.Generic;

namespace Bebis {

    public interface IPlayerSkillsLoadout {
        CharacterActionData NormalBtn1Skill { get; } // jump is usually expected here
        IReadOnlyList<CharacterActionData> NormalAttackSkills { get; }
        CharacterActionData NormalAerialAttackSkill { get; }
        CharacterActionData SecondaryAttackSkill { get; }
        CharacterActionData SecondaryAerialAttackSkill { get; }
        CharacterActionData NormalBtn4Skill { get; } // interact is usually expected here
        CharacterActionData RightTriggerSkill { get; }
        CharacterActionData LeftTriggerSkill  { get; } // defensive action is usually expected here

        CharacterActionData SkillSet1Btn1Skill { get; }
        CharacterActionData SkillSet1Btn2Skill { get; }
        CharacterActionData SkillSet1Btn3Skill { get; }
        CharacterActionData SkillSet1Btn4Skill { get; }

        CharacterActionData SkillSet2Btn1Skill { get; }
        CharacterActionData SkillSet2Btn2Skill { get; }
        CharacterActionData SkillSet2Btn3Skill { get; }
        CharacterActionData SkillSet2Btn4Skill { get; }
    }

    [System.Serializable]
    public class HackPlayerSkillsLoadout : IPlayerSkillsLoadout{
        [SerializeField] private CharacterActionData _normalBtn1Skill;
        [SerializeField] private List<CharacterActionData> _normalAttackSkills = new List<CharacterActionData>();
        [SerializeField] private CharacterActionData _normalAerialAttackSkill;
        [SerializeField] private CharacterActionData _secondaryAttackSkill;
        [SerializeField] private CharacterActionData _secondaryAerialAttackSkill;
        [SerializeField] private CharacterActionData _normalBtn4Skill;
        [SerializeField] private CharacterActionData _rightTriggerSkill;
        [SerializeField] private CharacterActionData _leftTriggerSkill;

        [SerializeField] private CharacterActionData _skillSet1Btn1Skill;
        [SerializeField] private CharacterActionData _skillSet1Btn2Skill;
        [SerializeField] private CharacterActionData _skillSet1Btn3Skill;
        [SerializeField] private CharacterActionData _skillSet1Btn4Skill;

        [SerializeField] private CharacterActionData _skillSet2Btn1Skill;
        [SerializeField] private CharacterActionData _skillSet2Btn2Skill;
        [SerializeField] private CharacterActionData _skillSet2Btn3Skill;
        [SerializeField] private CharacterActionData _skillSet2Btn4Skill;

        public CharacterActionData NormalBtn1Skill => _normalBtn1Skill;
        public IReadOnlyList<CharacterActionData> NormalAttackSkills => _normalAttackSkills;
        public CharacterActionData NormalAerialAttackSkill => _normalAerialAttackSkill;
        public CharacterActionData SecondaryAttackSkill => _secondaryAttackSkill;
        public CharacterActionData SecondaryAerialAttackSkill => _secondaryAerialAttackSkill;
        public CharacterActionData NormalBtn4Skill => _normalBtn4Skill;
        public CharacterActionData RightTriggerSkill => _rightTriggerSkill;
        public CharacterActionData LeftTriggerSkill => _leftTriggerSkill;

        public CharacterActionData SkillSet1Btn1Skill => _skillSet1Btn1Skill;
        public CharacterActionData SkillSet1Btn2Skill => _skillSet1Btn2Skill;
        public CharacterActionData SkillSet1Btn3Skill => _skillSet1Btn3Skill;
        public CharacterActionData SkillSet1Btn4Skill => _skillSet1Btn4Skill;

        public CharacterActionData SkillSet2Btn1Skill => _skillSet2Btn1Skill;
        public CharacterActionData SkillSet2Btn2Skill => _skillSet2Btn2Skill;
        public CharacterActionData SkillSet2Btn3Skill => _skillSet2Btn3Skill;
        public CharacterActionData SkillSet2Btn4Skill => _skillSet2Btn4Skill;
    }
}
