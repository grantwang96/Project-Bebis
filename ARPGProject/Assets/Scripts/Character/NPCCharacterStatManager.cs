using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class NPCCharacterStatManager : MonoBehaviour, ICharacterStatManager {

        public CharacterStats BaseStats { get; } // updated rarely (ex. level up). Used to build FinalStats
        public CharacterStats FinalStats { get; } // updated frequently (ex. equipment change)
        public CharacterTag CharacterTags => _characterTags;

        public int Attack { get; }
        public int Defense { get; }

        public event Action<CharacterStats> OnBaseStatsUpdated;
        public event Action<CharacterStats> OnFinalStatsUpdated;

        [SerializeField] private CharacterTag _characterTags;
    }
}
