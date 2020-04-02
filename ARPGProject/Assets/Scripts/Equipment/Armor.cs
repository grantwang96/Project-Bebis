using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    [CreateAssetMenu (menuName = "Equipment/Armor")]
    public class Armor : ScriptableObject, IEquipment {

        [SerializeField] private EquipmentType _equipmentType;
        public EquipmentType EquipmentType => _equipmentType;
        [SerializeField] private CharacterStats _stats;
        public CharacterStats Stats => _stats;
        [SerializeField] private CharacterStatModifiers _modifiers;
        public CharacterStatModifiers Modifiers => _modifiers;

        public CharacterStats ApplyStats(CharacterStats stats) {
            return stats + _stats;
        }

        public CharacterStatModifiers ApplyModifiers(CharacterStatModifiers modifiers) {
            return modifiers + _modifiers;
        }
    }
}
