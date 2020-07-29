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
        public readonly Vector3 KnockBack;
        public readonly float Force;
        public readonly bool OverrideForce;
        public readonly ICharacter Attacker;

        public HitEventInfo(
            int power,
            Vector3 knockBack,
            float force,
            bool overrideForce,
            ICharacter attacker
            ) {
            Power = power;
            KnockBack = knockBack;
            Force = force;
            OverrideForce = overrideForce;
            Attacker = attacker;
        }
    }
}
