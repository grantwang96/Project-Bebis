using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IActionControllerV2 : ICharacterComponent
    {
        ICharacterActionStateV2 CurrentState { get; }

        event Action<ICharacterActionStateV2> OnCurrentStateUpdated;
        event Action<ICharacterActionStateV2> OnActionStateUpdated; // when the current action has been updated
    }
}
