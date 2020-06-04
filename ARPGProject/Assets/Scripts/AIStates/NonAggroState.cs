using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public class NonAggroState : AIState {

        [SerializeField] private AIState _onTakeDamage;

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
            // change to the take damage state
            // FireReadyToChangeState(_onTakeDamage);
        }
    }
}
