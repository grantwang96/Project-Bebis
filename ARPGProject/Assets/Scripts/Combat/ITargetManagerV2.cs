using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    // interface for character vision and target management system
    public interface ITargetManagerV2 : ICharacterComponent
    {
        ICharacterV2 CurrentTarget { get; }
    }
    
    // interface for objects that target managers can detect. This can include characters, dead bodies, etc.
    public interface IDetectable
    {
        DetectableTags DetectableTags { get; }// tags that describe this detectable
        Transform Transform { get; } // the actual gameobject to detect
    }

    [System.Flags]
    public enum DetectableTags
    {
        None = 0,
        Character = (1 << 0),
        CrimeScene = (2 << 0), // dead body for example
        WeaponMark = (3 << 0), // evidence that a weapon was used here (ex. stuck arrow)
        WeaponNoise = (4 << 0), // when a weapon collides with something
    }
}
