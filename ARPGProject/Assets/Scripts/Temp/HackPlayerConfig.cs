using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [System.Serializable]
    public class HackPlayerConfig {
        [SerializeField] public CharacterActionData JumpAction;
        [SerializeField] public List<AttackActionData> NormalAttack;
        [SerializeField] public AttackActionData SecondaryAttack;
        [SerializeField] public CharacterStats BaseStats;
    }
}
