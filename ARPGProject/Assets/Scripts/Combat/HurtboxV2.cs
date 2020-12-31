using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HurtboxV2 : MonoBehaviour
    {
        public ICharacterV2 Character { get; private set; }
        public HurtBoxState HurtBoxState => _hurtBoxState;

        [SerializeField] private HurtBoxState _hurtBoxState;

        public event Action<HurtboxInteractionV2> OnInteraction;

        public void Initialize(ICharacterV2 character) {
            if (character == null) {
                Debug.LogWarning($"[{name}]Initializing with null [{nameof(ICharacterV2)}]!");
            }
            Character = character;
        }

        public bool SendHitEvent(ICharacterV2 other, HitboxV2 hitbox, Action<ICharacterV2> OnInteractionSuccess, Action<ICharacterV2> OnInteractionFail = null) {
            if (!ShouldHit()) {
                return false;
            }
            HurtboxInteractionV2 interaction = new HurtboxInteractionV2() {
                OtherCharacter = other,
                Hurtbox = this,
                Hitbox = hitbox,
                OnInteractionSuccess = OnInteractionSuccess,
                OnInteractionFail = OnInteractionFail
            };
            OnInteraction?.Invoke(interaction);
            return true;
        }

        private bool ShouldHit() {
            return enabled;
        }
    }

    public class HurtboxInteractionV2
    {
        public ICharacterV2 OtherCharacter;
        public HurtboxV2 Hurtbox;
        public HitboxV2 Hitbox;
        public Action<ICharacterV2> OnInteractionSuccess;
        public Action<ICharacterV2> OnInteractionFail;
        public ICharacterActionStateV2 PerformingAction;
    }
}
