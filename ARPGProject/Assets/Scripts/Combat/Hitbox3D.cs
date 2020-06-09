using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class Hitbox3D : Hitbox {

        [SerializeField] private CombatHitboxInfo3D _info;

        public override void Initialize(HitboxInfo info) {
            CombatHitboxInfo3D info3D = info as CombatHitboxInfo3D;
            if(info3D == null) {
                CustomLogger.Error(name, $"Hitbox was given wrong type of hitbox info!");
                return;
            }
            _info = info3D;
            FireInitializedEvent();
        }

        private void OnTriggerEnter(Collider collider) {
            _info?.HitboxTriggered(this, collider);
            OnHit();
        }

        private void OnHit() {
            // do hitting collider effects here?
        }
    }

    public class HitboxInfo3D : HitboxInfo {

        public readonly Action<Hitbox3D, Collider> OnHitboxTriggered;

        public HitboxInfo3D(Action<Hitbox3D, Collider> onHitboxTriggered) {
            OnHitboxTriggered = onHitboxTriggered;
        }

        public void HitboxTriggered(Hitbox3D hitBox, Collider collider) {
            OnHitboxTriggered?.Invoke(hitBox, collider);
        }
    }
}

