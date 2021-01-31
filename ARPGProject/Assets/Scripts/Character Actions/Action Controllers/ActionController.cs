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
        Ready,
        Started,
        InProgress,
        CanTransition,
        Completed,
        CanBufferInput
    }
}
