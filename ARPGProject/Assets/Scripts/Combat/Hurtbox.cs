using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public enum HurtBoxState {
        Normal,
        Defending,
        Invulnerable
    }
    public abstract class Hurtbox : MonoBehaviour {

        public ICharacter Character { get; private set; }
        public event Action<HurtboxInteraction> OnHit;
        public event Action<HurtboxInteraction> OnDefend;

        [SerializeField] private HurtBoxState _hurtBoxState;
        public HurtBoxState HurtBoxState => _hurtBoxState;

        public virtual void Initialize(ICharacter character) {
            if (character == null) {
                CustomLogger.Warn(nameof(Hurtbox), $"Initializing with null [{nameof(ICharacter)}]");
            }
            Character = character;
        }
        
        public bool SendHitEvent(ICharacter other, Hitbox hitbox, Action<ICharacter> OnInteractionSuccess, Action<ICharacter> OnInteractionFail = null) {
            if (!ShouldHit()) {
                return false;
            }
            HurtboxInteraction interaction = new HurtboxInteraction() {
                OtherCharacter = other,
                Hurtbox = this,
                Hitbox = hitbox,
                OnInteractionSuccess = OnInteractionSuccess,
                OnInteractionFail = OnInteractionFail
            };
            switch (_hurtBoxState) {
                case HurtBoxState.Normal:
                    OnHit?.Invoke(interaction);
                    return true;
                case HurtBoxState.Defending:
                    OnDefend?.Invoke(interaction);
                    return true;
                default:
                    // do invulnerable hit fx
                    return false;
            }
        }

        private bool ShouldHit() {
            return enabled;
        }
    }

    public class HurtboxInteraction {
        public ICharacter OtherCharacter;
        public Hurtbox Hurtbox;
        public Hitbox Hitbox;
        public Action<ICharacter> OnInteractionSuccess;
        public Action<ICharacter> OnInteractionFail;
    }
}
