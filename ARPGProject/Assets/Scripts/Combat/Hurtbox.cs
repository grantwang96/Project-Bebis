using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class Hurtbox : MonoBehaviour {

        private IDamageable _damageable;

        public event Action<HitEventInfo> OnHit;

        public void Initialize(IDamageable damageable) {
            _damageable = damageable;
        }

        public void Hit(HitEventInfo info) {
            OnHit?.Invoke(info);
        }
    }
}
