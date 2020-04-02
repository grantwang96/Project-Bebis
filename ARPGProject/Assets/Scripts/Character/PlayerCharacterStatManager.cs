using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacterStatManager : PlayerCharacterComponent, ICharacterStatManager {

        public CharacterStats BaseStats { get; private set; }
        public CharacterStats FinalStats { get; private set; }

        public event Action<CharacterStats> OnBaseStatsUpdated;
        public event Action<CharacterStats> OnFinalStatsUpdated;

        private CharacterStatModifiers _modifiers = CharacterStatModifiers.Standard;
        private EquipmentManager _equipmentManager;

        public PlayerCharacterStatManager(PlayerCharacter character, EquipmentManager equipmentManager) : base(character) {
            BaseStats = character.HackConfig.BaseStats;
            FinalStats = BaseStats;
            _equipmentManager = equipmentManager;

            _equipmentManager.OnEquipmentUpdated += OnEquipmentUpdated;
        }

        private void OnEquipmentUpdated() {
            FinalStats = BaseStats;
            FinalStats += _equipmentManager.HelmetSlot?.Stats ?? new CharacterStats();
            FinalStats += _equipmentManager.TorsoSlot?.Stats ?? new CharacterStats();
            FinalStats += _equipmentManager.LeggingsSlot?.Stats ?? new CharacterStats();

            _modifiers = CharacterStatModifiers.Standard;
            _modifiers += _equipmentManager.HelmetSlot?.Modifiers ?? new CharacterStatModifiers();
            _modifiers += _equipmentManager.TorsoSlot?.Modifiers ?? new CharacterStatModifiers();
            _modifiers += _equipmentManager.LeggingsSlot?.Modifiers ?? new CharacterStatModifiers();

            FinalStats = CharacterStats.ApplyModifiers(FinalStats, _modifiers);
            OnFinalStatsUpdated?.Invoke(FinalStats);
        }
    }
}
