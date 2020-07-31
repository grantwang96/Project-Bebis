using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    // class that will contain data about the player's stats, skills, equipment, etc. This layer will interact with persistence
    public class PlayerCharacterManager : MonoBehaviour {

        public static PlayerCharacterManager Instance { get; private set; }
        public CharacterStats BaseStats { get; private set; }
        public IPlayerSkillsLoadout SkillsLoadout { get; private set; }
        public event Action<IPlayerSkillsLoadout> OnPlayerSkillsLoadoutSet;

        [SerializeField] private CharacterStats _hackPlayerBaseStats;
        [SerializeField] private CharacterStats _hackPlayerModifiers;
        [SerializeField] private HackPlayerSkillsLoadout _hackSkillsLoadout; // used for debugging and until persistence is implemented

        private void Awake() {
            Instance = this;
            SkillsLoadout = _hackSkillsLoadout;
        }

        private void Start() {
            OnPlayerSkillsLoadoutSet?.Invoke(_hackSkillsLoadout); // SUUUUUUUUUUUUUUPER HACKS
        }
    }
}
