using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HumanDamageable : IDamageableV2
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsDead { get; private set; } = false;

        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
        }

        public void ReceiveHit(HitEventInfo hitEventInfo) {

        }

        public event Action<int> OnHealthChanged;
        public event Action<HitEventInfo> OnReceivedHit;
    }
}
