using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [System.Serializable]
    public class CombatHitBoxData {
        [SerializeField] private int _basePower;
        [SerializeField] private MinMax_Int _powerRange;
        [SerializeField] private Vector3 _knockbackAngle;
        [SerializeField] private float _knockbackForce;
        [SerializeField] private bool _overrideForce;

        public int BasePower => _basePower;
        public MinMax_Int PowerRange => _powerRange;
        public Vector3 KnockbackAngle => _knockbackAngle;
        public float KnockbackForce => _knockbackForce;
        public bool OverrideForce => _overrideForce;
    }

    public class HitEventInfo {

        public readonly int Power;
        public readonly Vector3 KnockBackDirection;
        public readonly float Force;
        public readonly bool OverrideForce;
        public readonly ICharacter Attacker;

        public HitEventInfo(
            int power,
            Vector3 knockBackDirection,
            float force,
            bool overrideForce,
            ICharacter attacker
            ) {
            Power = power;
            KnockBackDirection = knockBackDirection;
            Force = force;
            OverrideForce = overrideForce;
            Attacker = attacker;
        }
    }
}
