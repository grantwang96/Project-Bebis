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
        public bool Dead => _health <= 0;

        [SerializeField] private AnimationData _onHitStunAnimationData;

        public event Action<int> OnCurrentHealthChanged;
        public event Action<int> OnMaxHealthChanged;
        public event Action<HitEventInfo> OnHit;
        public event Action<HitEventInfo> OnHitStun;
        public event Action OnDefeated;

        [SerializeField] private HurtboxController _hurtBoxController;

        public void TakeDamage(HitEventInfo hitEventInfo) {
            // do nothing if this character is already dead
            if(_health <= 0) {
                return;
            }
            // perform checks based on hurt box controller's state
            _health -= hitEventInfo.Power;
            OnCurrentHealthChanged?.Invoke(_health);
            _character.MoveController.AddForce(hitEventInfo.KnockBackDirection, hitEventInfo.Force, hitEventInfo.OverrideForce);
            OnHit?.Invoke(hitEventInfo);
            if (_health > 0) {
                // temp: all hits deal hitstun
                OnHitStun?.Invoke(hitEventInfo);
            } else {
                OnDefeated?.Invoke();
            }
        }
    }
}
