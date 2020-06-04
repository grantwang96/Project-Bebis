using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class NPCDamageable : CharacterComponent, IDamageable {

        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;
        public int Health => _health;
        public int MaxHealth => _maxHealth;

        public event Action<HitEventInfo> OnHit;
        public event Action OnDefeated;

        [SerializeField] private HurtboxController _hurtBoxController;

        public void TakeDamage(HitEventInfo hitEventInfo) {
            // perform checks based on hurt box controller's state
            _health -= hitEventInfo.Power;
            _character.MoveController.AddForce(hitEventInfo.KnockBack, hitEventInfo.Force);
        }
    }
}
