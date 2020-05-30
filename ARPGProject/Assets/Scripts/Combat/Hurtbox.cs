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
        public event Action<string, Action<ICharacter>> OnHit;
        public event Action<string, Action<ICharacter>> OnDefend;

        [SerializeField] private HurtBoxState _hurtBoxState;
        public HurtBoxState HurtBoxState => _hurtBoxState;

        public void Initialize(ICharacter character) {
            if(character == null) {
                CustomLogger.Warn(nameof(Hurtbox), $"Initializing with null [{nameof(ICharacter)}]");
            }
            Character = character;
        }

        public bool SendHitEvent(Action<ICharacter> OnHitAction) {
            if (!enabled) {
                return false;
            }
            switch (_hurtBoxState) {
                case HurtBoxState.Normal:
                    OnHit?.Invoke(name, OnHitAction);
                    return true;
                case HurtBoxState.Defending:
                    OnDefend?.Invoke(name, OnHitAction);
                    return true;
                default:
                    // do invulnerable hit fx
                    return false;
            }
        }
    }
}
