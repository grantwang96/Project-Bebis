using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
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

        public HitEventInfo(
            int power,
            Vector3 knockBack,
            float force) {
            Power = power;
            KnockBack = knockBack;
            Force = force;
        }
    }
}
