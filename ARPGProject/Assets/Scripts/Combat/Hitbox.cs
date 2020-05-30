using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class Hitbox : MonoBehaviour {

        [SerializeField] private HitboxInfo _info;

        public void SetInfo(HitboxInfo info) {
            _info = info;
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            OnHit();
            _info.HitboxTriggered(this, collider);
        }

        private void OnHit() {
            // do hitting collider effects here?
        }
    }
    
    public class HitboxInfo {

        public readonly Action<Hitbox, Collider2D> OnHitboxTriggered;

        public HitboxInfo(Action<Hitbox, Collider2D> onHitboxTriggered) {
            OnHitboxTriggered = onHitboxTriggered;
        }

        public void HitboxTriggered(Hitbox hitBox, Collider2D collider) {
            OnHitboxTriggered?.Invoke(hitBox, collider);
        }
    }

    [System.Serializable]
    public class CombatHitBoxData {
        [SerializeField] private int _basePower;
        [SerializeField] private MinMax_Int _powerRange;
        [SerializeField] private float _knockbackAngle;
        [SerializeField] private float _knockbackForce;

        public int BasePower => _basePower;
        public MinMax_Int PowerRange => _powerRange;
        public float KnockbackAngle => _knockbackAngle;
        public float KnockbackForce => _knockbackForce;
    }

    public class HitEventInfo {

        public readonly int Power;
        public readonly Vector3 KnockBack;
        public readonly float Force;

        public HitEventInfo(Hitbox hitbox,
            int power,
            Vector3 knockBack,
            float force) {
            Power = power;
            KnockBack = knockBack;
            Force = force;
        }
    }
}
