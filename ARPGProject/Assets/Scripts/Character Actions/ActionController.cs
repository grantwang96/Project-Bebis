using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {

    public interface IActionController {
        ActionPermissions Permissions { get; }
        ICharacterActionState CurrentState { get; }

        bool PerformAction(CharacterActionData actionData, CharacterActionContext context);

        event Action<ActionStatus> OnActionStatusUpdated;
        event Action OnPerformActionSuccess;
    }

    public enum ActionStatus {
        Started = 1,
        InProgress = 1 << 1,
        CanTransition = 2 << 1,
        Completed = 3 << 1
    }
}
