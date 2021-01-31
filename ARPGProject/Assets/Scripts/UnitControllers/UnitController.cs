using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IUnitController : ICharacterComponent
    {
        ITargetManagerV2 TargetManager { get; }
        ICharacterData CharacterData { get; }

        Vector3 MovementInput { get; }
        Vector3 RotationInput { get; }

        event Action<ICharacterActionDataV2, CharacterActionContext> OnActionAttempted;
        event Action<IReadOnlyList<ICharacterActionDataV2>> OnRegisteredCharacterActionsUpdated;
    }
}
