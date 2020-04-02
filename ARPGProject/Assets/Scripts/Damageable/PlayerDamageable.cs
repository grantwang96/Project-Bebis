using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerDamageable : PlayerCharacterComponent, IDamageable
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public event Action<HitEventInfo> OnHit;
        public event Action OnDefeated;

        public PlayerDamageable(PlayerCharacter character) : base(character) {
            Hurtbox hurtBox = _character.GetComponent<Hurtbox>();
            hurtBox.OnHit += OnHurtboxHit;
        }

        private void OnHurtboxHit(HitEventInfo info) {

        }
    }
}
