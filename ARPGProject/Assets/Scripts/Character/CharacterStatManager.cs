using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public interface ICharacterStatManager {
        CharacterStats BaseStats { get; } // updated rarely (ex. level up). Used to build FinalStats
        CharacterStats FinalStats { get; } // updated frequently (ex. equipment change)
        CharacterTag CharacterTags { get; }

        int Attack { get; }
        int Defense { get; }

        event Action<CharacterStats> OnBaseStatsUpdated;
        event Action<CharacterStats> OnFinalStatsUpdated;
    }

    [System.Flags]
    public enum CharacterTag {
        Player = 1,
        Citizen = 1 << 1,
        Criminal = 2 << 1,
    }
}
