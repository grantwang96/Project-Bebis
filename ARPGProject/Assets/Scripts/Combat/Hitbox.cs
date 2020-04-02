using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class Hitbox : MonoBehaviour {

        [SerializeField] private HitboxInfo _info;
        [SerializeField] private Hurtbox _hurtBox;

        public void SetInfo(HitboxInfo info) {
            _info = info;
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            OnHit();
            Hurtbox hurtBox = collider.GetComponent<Hurtbox>();
            if (hurtBox == null || hurtBox == _hurtBox) {
                return;
            }
            HitEventInfo hitEvent = new HitEventInfo(this, _info.Power, CalculateRelativeDirection(), _info.KnockbackForce);
            hurtBox.Hit(hitEvent);
        }

        private Vector3 CalculateRelativeDirection() {
            return transform.TransformDirection(_info.KnockbackDirection).normalized;
        }

        private void OnHit() {
            // do hitting collider effects here?
        }
    }
    
    public class HitboxInfo {
        public readonly int Power;
        public readonly Vector3 KnockbackDirection;
        public readonly float KnockbackForce;

        public HitboxInfo(int power, Vector3 knockbackDirection, float force) {
            Power = power;
            KnockbackDirection = knockbackDirection;
            KnockbackForce = force;
        }
    }

    [System.Serializable]
    public class HitboxModifierInfo {
        [SerializeField] private int _basePower;
        [SerializeField] private MinMax_Int _powerRange;
        [SerializeField] private Vector3 _knockbackDirection;
        [SerializeField] private float _knockbackForce;

        public int BasePower => _basePower;
        public MinMax_Int PowerRange => _powerRange;
        public Vector3 KnockbackDirection => _knockbackDirection;
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
