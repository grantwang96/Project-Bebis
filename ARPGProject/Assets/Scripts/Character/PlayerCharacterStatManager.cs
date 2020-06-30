using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacterStatManager : MonoBehaviour, ICharacterStatManager {

        public CharacterStats BaseStats { get; private set; }
        public CharacterStats FinalStats { get; private set; }
        public CharacterTag CharacterTags => _characterTags;

        public int Attack { get; private set; }
        public int Defense { get; private set; }

        public event Action<CharacterStats> OnBaseStatsUpdated;
        public event Action<CharacterStats> OnFinalStatsUpdated;

        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private CharacterTag _characterTags;

        private CharacterStatModifiers _modifiers = CharacterStatModifiers.Standard;

        private void Start() {
            BaseStats = _playerCharacter.HackConfig.BaseStats;
            FinalStats = BaseStats;
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
