using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerDamageable : CharacterComponent, IDamageable
    {
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
        
        public void ReceiveHit(HitEventInfo hitEventInfo) {
            // perform checks based on hurt box controller's state
            HurtBoxState hurtBoxState = _character.HurtboxController.HitHurtboxState;
            switch (hurtBoxState) {
                case HurtBoxState.Normal:
                    OnNormalHit(hitEventInfo);
                    break;
                case HurtBoxState.Defending:
                    OnDefendedHit(hitEventInfo);
                    break;
            }

        }

        private void OnNormalHit(HitEventInfo hitEventInfo) {
            _health -= hitEventInfo.Power;
            OnCurrentHealthChanged?.Invoke(_health);
            _character.MoveController.AddForce(hitEventInfo.KnockBackDirection, hitEventInfo.Force);
            OnHit?.Invoke(hitEventInfo);
            if (_health > 0) {
                // temp: all hits deal hitstun
                ApplyHitStun(hitEventInfo);
            } else {
                OnDefeated?.Invoke();
            }
        }

        private void OnDefendedHit(HitEventInfo hitEventInfo) {
            // TODO: calculate reduced damage/stamina
            Debug.Log("Defended");
        }

        private void ApplyHitStun(HitEventInfo hitEventInfo) {
            _character.AnimationController.UpdateAnimationState(_onHitStunAnimationData);
            OnHitStun?.Invoke(hitEventInfo);
        }
    }
}
