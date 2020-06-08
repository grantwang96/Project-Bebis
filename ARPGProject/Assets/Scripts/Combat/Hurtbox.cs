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
    public class Hurtbox : MonoBehaviour {

        public ICharacter Character { get; private set; }
        public event Action<Hitbox, Action<ICharacter>> OnHit;
        public event Action<Hitbox, Action<ICharacter>> OnDefend;

        [SerializeField] private HurtBoxState _hurtBoxState;
        public HurtBoxState HurtBoxState => _hurtBoxState;

        public void Initialize(ICharacter character) {
            if(character == null) {
                CustomLogger.Warn(nameof(Hurtbox), $"Initializing with null [{nameof(ICharacter)}]");
            }
            Character = character;
        }

        public bool SendHitEvent(Hitbox hitbox, Action<ICharacter> OnHitAction) {
            if (!enabled) {
                return false;
            }
            switch (_hurtBoxState) {
                case HurtBoxState.Normal:
                    OnHit?.Invoke(hitbox, OnHitAction);
                    return true;
                case HurtBoxState.Defending:
                    OnDefend?.Invoke(hitbox, OnHitAction);
                    return true;
                default:
                    // do invulnerable hit fx
                    return false;
            }
        }
    }
}
