using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [System.Serializable]
    public class HackPlayerConfig {
        [SerializeField] public CharacterActionData InteractAction;
        [SerializeField] public CharacterActionData JumpAction;
        [SerializeField] public List<CharacterActionData> NormalAttack;
        [SerializeField] public CharacterActionData SecondaryAttack;
        [SerializeField] public CharacterActionData SkillMode1;
        [SerializeField] public CharacterActionData SkillMode2;
        [SerializeField] public CharacterStats BaseStats;
    }
}
