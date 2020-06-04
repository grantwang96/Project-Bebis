using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public abstract class DamageableState : AIState {

        [SerializeField] protected AIState _onTakeDamage;

        public override void Enter() {
            base.Enter();
            _character.Damageable.OnHit += OnTakeDamage;
        }

        public override void Exit(AIState nextState) {
            base.Exit(nextState);
            _character.Damageable.OnHit -= OnTakeDamage;
        }

        protected virtual void OnTakeDamage(HitEventInfo info) {
            // do take damage things
        }
    }
}
