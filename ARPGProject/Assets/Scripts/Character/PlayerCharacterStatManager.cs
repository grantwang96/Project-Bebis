using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacterStatManager : PlayerCharacterComponent, ICharacterStatManager {

        public CharacterStats BaseStats { get; private set; }
        public CharacterStats FinalStats { get; private set; }

        public int Attack { get; private set; }
        public int Defense { get; private set; }

        public event Action<CharacterStats> OnBaseStatsUpdated;
        public event Action<CharacterStats> OnFinalStatsUpdated;

        private CharacterStatModifiers _modifiers = CharacterStatModifiers.Standard;
        private EquipmentManager _equipmentManager;

        public PlayerCharacterStatManager(PlayerCharacter character, EquipmentManager equipmentManager) : base(character) {
            BaseStats = character.HackConfig.BaseStats;
            FinalStats = BaseStats;
            _equipmentManager = equipmentManager;

            _equipmentManager.OnEquipmentUpdated += OnEquipmentUpdated;
            OnEquipmentUpdated(null);
        }

        private void OnEquipmentUpdated(IEquipment equipment) {
            FinalStats = BaseStats;

            // TODO: update attack/defense power based on equipment
            // temp hack for now
            Attack = FinalStats.Strength;
            Defense = FinalStats.Fortitude;

            ApplyEquipmentStats(equipment);
            ApplyEquipmentModifiers(equipment);

            FinalStats = CharacterStats.ApplyModifiers(FinalStats, _modifiers);

            OnFinalStatsUpdated?.Invoke(FinalStats);
        }

        private void ApplyEquipmentStats(IEquipment equipment) {
            FinalStats = equipment?.ApplyStats(FinalStats) ?? FinalStats;
            Attack += equipment?.Attack ?? 0;
            Defense += equipment?.Defense ?? 0;
        }

        private void ApplyEquipmentModifiers(IEquipment equipment) {
            _modifiers = equipment?.ApplyModifiers(_modifiers) ?? _modifiers;
        }
    }
}
