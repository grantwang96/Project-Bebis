﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    [CreateAssetMenu (menuName = "Equipment/Armor")]
    public class Armor : ScriptableObject, IEquipment {

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
        // [SerializeField] private List<AnimationClipOverride> _runOverrides = new List<AnimationClipOverride>();
        // add new clip overrides as necessary

        public CharacterStats ApplyStats(CharacterStats stats) {
            return stats + _stats;
        }

        public CharacterStatModifiers ApplyModifiers(CharacterStatModifiers modifiers) {
            return modifiers + _modifiers;
        }
    }
}
