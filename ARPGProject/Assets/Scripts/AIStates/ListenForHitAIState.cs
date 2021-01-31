using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class ListenForHitAIState : AIStateV2
    {
        public const string Name = "ListenForHit";

        [SerializeField] private AIStateV2 _onHitState;

        protected override void OnEnter() {
            base.OnEnter();
            // start listening for damage events
            _character.Damageable.OnReceivedHit += OnReceivedHit;
        }

        protected override void OnExit() {
            base.OnExit();
            // stop listening for damage events
            _character.Damageable.OnReceivedHit -= OnReceivedHit;
        }

        private void OnReceivedHit(HitEventInfoV2 hitEventInfo) {
            FireReadyToTransition(_onHitState);
        }
    }
}
