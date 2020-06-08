using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public abstract class Hitbox : MonoBehaviour {

        public event Action<Hitbox> OnHitboxInitialized;

        public abstract void Initialize(HitboxInfo hitBoxInfo);

        protected void FireInitializedEvent() {
            OnHitboxInitialized?.Invoke(this);
        }
    }

    public abstract class HitboxInfo {

    }
}
