using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HumanDamageable : ICharacterDamageable
    {
        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public bool IsDead { get; private set; } = false;

        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
        }

        public void ReceiveHit(HitEventInfoV2 hitEventInfo) {
            if(hitEventInfo.Hurtbox == null) {
                Debug.LogWarning("Hit hurt box was null!");
                return;
            }
            switch (hitEventInfo.Hurtbox.HurtBoxState) {
                case HurtBoxState.Normal:
                    OnNormalHit(hitEventInfo);
                    break;
                default:
                    break;
            }
            OnReceivedHit?.Invoke(hitEventInfo);
        }

        private void OnNormalHit(HitEventInfoV2 hitEventInfo) {
            Health -= hitEventInfo.Power;
        }

        public event Action<int> OnHealthChanged;
        public event Action<HitEventInfoV2> OnReceivedHit;
    }
}
