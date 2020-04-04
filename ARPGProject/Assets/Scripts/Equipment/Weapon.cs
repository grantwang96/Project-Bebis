using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    [CreateAssetMenu(menuName = "Equipment/Weapon")]
    public class Weapon : ScriptableObject, IEquipment {

        [SerializeField] private int _attack;
        public int Attack => _attack;
        [SerializeField] private int _defense;
        public int Defense => _defense;

        [SerializeField] private EquipmentType _equipmentType;
        public EquipmentType EquipmentType => _equipmentType;
        [SerializeField] private CharacterStats _stats;
        public CharacterStats Stats => _stats;
        [SerializeField] private CharacterStatModifiers _modifiers;
        public CharacterStatModifiers Modifiers => _modifiers;

        [SerializeField] private List<AnimationClipOverride> _animationOverrides = new List<AnimationClipOverride>();
        public IReadOnlyList<AnimationClipOverride> AnimationOverrides => _animationOverrides;

        public CharacterStats ApplyStats(CharacterStats stats) {
            return _stats + stats;
        }

        public CharacterStatModifiers ApplyModifiers(CharacterStatModifiers modifiers) {
            return _modifiers + modifiers;
        }

    }
}
