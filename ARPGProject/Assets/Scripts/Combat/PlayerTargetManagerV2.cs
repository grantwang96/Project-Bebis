using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class PlayerTargetManagerV2 : ITargetManagerV2
    {
        public ICharacterV2 CurrentTarget { get; private set; } // current character this target manager is focused on
        public event Action OnCurrentTargetUpdated;

        // the character to this object
        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
        }

        public void OverrideCurrentTarget(ICharacterV2 character) {
            CurrentTarget = character;
            OnCurrentTargetUpdated?.Invoke();
        }

        public void ClearCurrentTarget() {
            CurrentTarget = null;
            OnCurrentTargetUpdated?.Invoke();
        }
    }
}
