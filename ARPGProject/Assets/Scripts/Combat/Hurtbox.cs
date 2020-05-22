using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class Hurtbox : MonoBehaviour {

        public ICharacter Character { get; private set; }

        public event Action<string, HitEventInfo> OnHit;

        public void Initialize(ICharacter character) {
            if(character == null) {
                CustomLogger.Warn(nameof(Hurtbox), $"Initializing with null [{nameof(ICharacter)}]");
            }
            Character = character;
        }

        public void SendHitEvent(HitEventInfo hitEventInfo) {
            OnHit?.Invoke(name, hitEventInfo);
        }
    }
}
