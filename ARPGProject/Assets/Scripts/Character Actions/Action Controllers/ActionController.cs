using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {

    public interface IActionController {
        ICharacterActionState CurrentState { get; }

        event Action OnCurrentActionUpdated;
    }

    public interface IActionControllerInfoProvider
    {

        event Action<ICharacterActionData, CharacterActionContext> OnActionAttempted;
    }

    public enum ActionStatus {
        Started = 1,
        InProgress = 1 << 1,
        CanTransition = 2 << 1,
        Completed = 3 << 1,
        Ready = 4 << 1,
        CanBufferInput = 5 << 1
    }
}
