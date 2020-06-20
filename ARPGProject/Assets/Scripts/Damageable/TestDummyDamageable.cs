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
        public event Action<HitEventInfo> OnHitStun;
        public event Action OnDefeated;

        private void Awake() {
            _moveController = _moveControllerGO.GetComponent<IMoveController>();
        }

        public void TakeDamage(HitEventInfo info) {
            switch (_hurtboxController.HitHurtboxState) {
                case HurtBoxState.Normal:
                    CustomLogger.Log(nameof(TestDummyDamageable), $"Got hit for {info.Power} damage!");
                    break;
                case HurtBoxState.Defending:
                    CustomLogger.Log(nameof(TestDummyDamageable), $"Defended {info.Power} damage!");
                    break;
            }
            _moveController.AddForce(info.KnockBack, info.Force, true);
        }
    }
}
