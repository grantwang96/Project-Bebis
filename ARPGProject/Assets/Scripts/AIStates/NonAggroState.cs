using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public class NonAggroState : AIState {

        [SerializeField] private AIState _onTakeDamageHostileFound;
        [SerializeField] private AIState _onTakeDamageHostileHidden;
        [SerializeField] private float _hostileHiddenStartingDetection;
        [SerializeField] private NPCTargetManager _npcTargetManager;

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
            float detectionValue = _hostileHiddenStartingDetection;
            AIState nextState = _onTakeDamageHostileHidden;
            // if the target is visible
            if(_npcTargetManager.TargetWithinRange(info.Attacker) && _npcTargetManager.TargetVisible(info.Attacker)) {
                detectionValue = 1f + NPCTargetManager.AttentionBuffer;
                nextState = _onTakeDamageHostileFound;
            }
            _npcTargetManager.RegisterOrUpdateHostile(info.Attacker, detectionValue);
            FireReadyToChangeState(nextState);
        }
    }
}
