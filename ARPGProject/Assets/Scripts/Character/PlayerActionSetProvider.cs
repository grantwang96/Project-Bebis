using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface IPlayerActionSetProvider {

        ICharacterActionData Jump { get; }
        ICharacterActionData NormalAttack { get; }
        ICharacterActionData SpecialAttack { get; }
        ICharacterActionData Interact { get; }

        ICharacterActionData LeftShoulderSkill { get; }
        ICharacterActionData RightShoulderSkill { get; }

        ICharacterActionData SkillSet1South { get; }
        ICharacterActionData SkillSet1West { get; }
        ICharacterActionData SkillSet1North { get; }
        ICharacterActionData SkillSet1East { get; }

        ICharacterActionData SkillSet2South { get; }
        ICharacterActionData SkillSet2West { get; }
        ICharacterActionData SkillSet2North { get; }
        ICharacterActionData SkillSet2East { get; }
    }

    public class PlayerActionSetProvider : MonoBehaviour, IPlayerActionSetProvider {

        public ICharacterActionData Jump => _jump;
        public ICharacterActionData NormalAttack => _normalAttack;
        public ICharacterActionData SpecialAttack => _specialAttack;
        public ICharacterActionData Interact => _interact;

        public ICharacterActionData RightShoulderSkill => _skillSet1;
        public ICharacterActionData LeftShoulderSkill => _skillSet2;

        public ICharacterActionData SkillSet1South => _set1South;
        public ICharacterActionData SkillSet1West => _set1West;
        public ICharacterActionData SkillSet1North => _set1North;
        public ICharacterActionData SkillSet1East => _set1East;

        public ICharacterActionData SkillSet2South => _set2South;
        public ICharacterActionData SkillSet2West => _set2West;
        public ICharacterActionData SkillSet2North => _set2North;
        public ICharacterActionData SkillSet2East => _set2East;

        [SerializeField] private CharacterActionData _jump;
        [SerializeField] private CharacterActionData _normalAttack;
        [SerializeField] private CharacterActionData _specialAttack;
        [SerializeField] private CharacterActionData _interact;

        [SerializeField] private CharacterActionData _skillSet1;
        [SerializeField] private CharacterActionData _skillSet2;

        [SerializeField] private CharacterActionData _set1South;
        [SerializeField] private CharacterActionData _set1West;
        [SerializeField] private CharacterActionData _set1North;
        [SerializeField] private CharacterActionData _set1East;

        [SerializeField] private CharacterActionData _set2South;
        [SerializeField] private CharacterActionData _set2West;
        [SerializeField] private CharacterActionData _set2North;
        [SerializeField] private CharacterActionData _set2East;

        public void Initialize() {
            
        }

        public void SetCharacterActions() {

        }
    }
}
