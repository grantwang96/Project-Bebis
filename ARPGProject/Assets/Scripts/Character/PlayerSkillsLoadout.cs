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

    public interface IPlayerSkillsLoadoutV2
    {
        CharacterActionDataV2 NormalBtn1Skill { get; } // jump is usually expected here
        IReadOnlyList<CharacterActionDataV2> NormalAttackSkills { get; }
        CharacterActionDataV2 NormalAerialAttackSkill { get; }
        CharacterActionDataV2 SecondaryAttackSkill { get; }
        CharacterActionDataV2 SecondaryAerialAttackSkill { get; }
        CharacterActionDataV2 NormalBtn4Skill { get; } // interact is usually expected here
        CharacterActionDataV2 RightTriggerSkill { get; }
        CharacterActionDataV2 LeftTriggerSkill { get; } // defensive action is usually expected here

        CharacterActionDataV2 SkillSet1Btn1Skill { get; }
        CharacterActionDataV2 SkillSet1Btn2Skill { get; }
        CharacterActionDataV2 SkillSet1Btn3Skill { get; }
        CharacterActionDataV2 SkillSet1Btn4Skill { get; }

        CharacterActionDataV2 SkillSet2Btn1Skill { get; }
        CharacterActionDataV2 SkillSet2Btn2Skill { get; }
        CharacterActionDataV2 SkillSet2Btn3Skill { get; }
        CharacterActionDataV2 SkillSet2Btn4Skill { get; }
    }

    [System.Serializable]
    public class HackPlayerSkillsLoadoutV2 : IPlayerSkillsLoadoutV2
    {
        [SerializeField] private CharacterActionDataV2 _normalBtn1Skill;
        [SerializeField] private List<CharacterActionDataV2> _normalAttackSkills = new List<CharacterActionDataV2>();
        [SerializeField] private CharacterActionDataV2 _normalAerialAttackSkill;
        [SerializeField] private CharacterActionDataV2 _secondaryAttackSkill;
        [SerializeField] private CharacterActionDataV2 _secondaryAerialAttackSkill;
        [SerializeField] private CharacterActionDataV2 _normalBtn4Skill;
        [SerializeField] private CharacterActionDataV2 _rightTriggerSkill;
        [SerializeField] private CharacterActionDataV2 _leftTriggerSkill;

        [SerializeField] private CharacterActionDataV2 _skillSet1Btn1Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet1Btn2Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet1Btn3Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet1Btn4Skill;

        [SerializeField] private CharacterActionDataV2 _skillSet2Btn1Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet2Btn2Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet2Btn3Skill;
        [SerializeField] private CharacterActionDataV2 _skillSet2Btn4Skill;

        public CharacterActionDataV2 NormalBtn1Skill => _normalBtn1Skill;
        public IReadOnlyList<CharacterActionDataV2> NormalAttackSkills => _normalAttackSkills;
        public CharacterActionDataV2 NormalAerialAttackSkill => _normalAerialAttackSkill;
        public CharacterActionDataV2 SecondaryAttackSkill => _secondaryAttackSkill;
        public CharacterActionDataV2 SecondaryAerialAttackSkill => _secondaryAerialAttackSkill;
        public CharacterActionDataV2 NormalBtn4Skill => _normalBtn4Skill;
        public CharacterActionDataV2 RightTriggerSkill => _rightTriggerSkill;
        public CharacterActionDataV2 LeftTriggerSkill => _leftTriggerSkill;

        public CharacterActionDataV2 SkillSet1Btn1Skill => _skillSet1Btn1Skill;
        public CharacterActionDataV2 SkillSet1Btn2Skill => _skillSet1Btn2Skill;
        public CharacterActionDataV2 SkillSet1Btn3Skill => _skillSet1Btn3Skill;
        public CharacterActionDataV2 SkillSet1Btn4Skill => _skillSet1Btn4Skill;

        public CharacterActionDataV2 SkillSet2Btn1Skill => _skillSet2Btn1Skill;
        public CharacterActionDataV2 SkillSet2Btn2Skill => _skillSet2Btn2Skill;
        public CharacterActionDataV2 SkillSet2Btn3Skill => _skillSet2Btn3Skill;
        public CharacterActionDataV2 SkillSet2Btn4Skill => _skillSet2Btn4Skill;
    }
}
