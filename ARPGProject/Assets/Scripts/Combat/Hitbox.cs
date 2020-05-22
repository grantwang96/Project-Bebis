using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class Hitbox : MonoBehaviour {

        [SerializeField] private HitboxInfo _info;
        [SerializeField] private HurtboxController _hurtboxController;

        public void SetInfo(HitboxInfo info) {
            _info = info;
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            OnHit();
            Hurtbox hurtBox = collider.GetComponent<Hurtbox>();
            if (hurtBox == null || _hurtboxController.Hurtboxes.ContainsKey(hurtBox.name)) {
                return;
            }
            HitEventInfo hitEvent = new HitEventInfo(this, _info.Power, CalculateRelativeDirection(), _info.KnockbackForce);
            hurtBox.Hit(hitEvent);
        }

        private Vector3 CalculateRelativeDirection() {
            return ExtraMath.Rotate(transform.up, _info.KnockbackAngle);
            // return transform.TransformDirection(_info.KnockbackAngle).normalized;
        }

        private void OnHit() {
            // do hitting collider effects here?
        }
    }
    
    public class HitboxInfo {
        public readonly int Power;
        public readonly float KnockbackAngle;
        public readonly float KnockbackForce;

        public HitboxInfo(int power, float knockbackAngle, float force) {
            Power = power;
            KnockbackAngle = knockbackAngle;
            KnockbackForce = force;
        }
    }

    [System.Serializable]
    public class HitboxModifierInfo {
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
