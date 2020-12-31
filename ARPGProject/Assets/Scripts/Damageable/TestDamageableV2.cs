using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class TestDamageableV2 : MonoBehaviour, IDamageableV2
    {
        public int Health => _health;
        public int MaxHealth => _maxHealth;
        public bool IsDead => _isDead;

        public event Action<int> OnHealthChanged;
        public event Action<HitEventInfoV2> OnReceivedHit;

        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;
        [SerializeField] private bool _isDead;

        public void ReceiveHit(HitEventInfoV2 hitEventInfo) {
            Debug.Log($"[{name}]: Received hit for: {hitEventInfo.Power} damage!");
            _health -= hitEventInfo.Power;
            OnHealthChanged?.Invoke(_health);
        }
    }
}
