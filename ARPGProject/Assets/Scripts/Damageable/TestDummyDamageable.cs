using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummyDamageable : MonoBehaviour, IDamageable {

        [SerializeField] private GameObject _moveControllerGO;
        [SerializeField] private HurtboxController _hurtboxController;
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;

        private IMoveController _moveController;

        public int Health => _health;
        public int MaxHealth => _maxHealth;

        public event Action<HitEventInfo> OnHit;
        public event Action OnDefeated;

        private void Awake() {
            _moveController = _moveControllerGO.GetComponent<IMoveController>();
            _hurtboxController.OnHit += OnHurtboxHit;
        }

        private void OnHurtboxHit(HitEventInfo info) {
            CustomLogger.Log(nameof(TestDummyDamageable), $"Got hit for {info.Power} damage!");
            _moveController.AddForce(info.KnockBack, info.Force, true);
        }
    }
}
