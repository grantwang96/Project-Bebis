using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IUnitController : ICharacterComponent
    {
        Vector3 MovementInput { get; }
        Vector3 RotationInput { get; }

        event Action<ICharacterActionDataV2, CharacterActionContext> OnActionAttempted;
        event Action OnRegisteredCharacterActionsUpdated;
    }
}
