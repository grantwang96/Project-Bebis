using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public interface ICharacterStatManager {
        CharacterStats BaseStats { get; } // updated rarely (ex. level up). Used to build FinalStats
        CharacterStats FinalStats { get; } // updated frequently (ex. equipment change)

        event Action<CharacterStats> OnBaseStatsUpdated;
        event Action<CharacterStats> OnFinalStatsUpdated;
    }
}
