using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IDamageableV2 : ICharacterComponent
    {
        int Health { get; }
        int MaxHealth { get; }
        bool IsDead { get; }

        void ReceiveHit(HitEventInfo hitEventInfo); // message is sent here to determine if a hit has been received

        event Action<int> OnHealthChanged;
        event Action<HitEventInfo> OnReceivedHit;
    }
}
