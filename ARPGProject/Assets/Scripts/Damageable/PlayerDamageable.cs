using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;
        public int Health => _health;
        public int MaxHealth => _maxHealth;

        public event Action<HitEventInfo> OnHit;
        public event Action OnDefeated;

        [SerializeField] private HurtboxController _hurtBoxController;

        private void Awake() {
            _hurtBoxController.OnHit += OnHurtboxHit;
        }

        private void OnDestroy() {
            _hurtBoxController.OnHit -= OnHurtboxHit;
        }

        private void OnHurtboxHit(HitEventInfo info) {
            _health -= info.Power;
        }
    }
}
