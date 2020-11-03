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

    public struct HitEventInfo {
        public int Power;
        public Vector3 KnockBackDirection;
        public float Force;
        public bool OverrideForce;
        public ICharacter Attacker;
        public Hitbox Hitbox;
        public Hurtbox Hurtbox;
    }
}
