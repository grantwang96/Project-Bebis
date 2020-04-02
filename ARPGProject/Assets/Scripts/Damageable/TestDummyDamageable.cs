using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummyDamageable : IDamageable {

        private IMoveController _moveController;

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public event Action<HitEventInfo> OnHit;
        public event Action OnDefeated;

        public TestDummyDamageable(Hurtbox hurtBox, IMoveController moveController) {
            hurtBox.OnHit += Hit;
            _moveController = moveController;
        }

        private void Hit(HitEventInfo info) {
            CustomLogger.Log(nameof(TestDummyDamageable), $"Got hit for {info.Power} damage!");
            _moveController.AddForce(info.KnockBack, info.Force, true);
        }
    }
}
