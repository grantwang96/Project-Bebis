using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class Hitbox2D : Hitbox {

        [SerializeField] private HitboxInfo2D _info;

        public override void Initialize(HitboxInfo info) {
            HitboxInfo2D info2D = info as HitboxInfo2D;
            if(info2D == null) {
                CustomLogger.Error(name, $"Hitbox was given wrong type of hitbox info!");
                return;
            }
            _info = info2D;
            FireInitializedEvent();
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            _info.HitboxTriggered(this, collider);
            OnHit();
        }

        private void OnHit() {
            // do hitting collider effects here?
        }
    }
    
    public class HitboxInfo2D : HitboxInfo{

        public readonly Action<Hitbox2D, Collider2D> OnHitboxTriggered;

        public HitboxInfo2D(Action<Hitbox2D, Collider2D> onHitboxTriggered) {
            OnHitboxTriggered = onHitboxTriggered;
        }

        public void HitboxTriggered(Hitbox2D hitBox, Collider2D collider) {
            OnHitboxTriggered?.Invoke(hitBox, collider);
        }
    }
}
