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
        Transform Transform { get; } // the actual gameobject to detect
        
    }
}
