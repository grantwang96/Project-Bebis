using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public abstract class PlayerCharacter : MonoBehaviour, ICharacter {

        public static ICharacter Instance { get; protected set; }

        [SerializeField] protected HitboxController _hitboxController;
        [SerializeField] protected HurtboxController _hurtboxController;

        public abstract IDamageable Damageable { get; }
        public abstract IMoveController MoveController { get; }
        public abstract IActionController ActionController { get; }
        public abstract IAnimationController AnimationController { get; }
        public abstract ICharacterStatManager CharacterStatManager { get; }
        public abstract ITargetManager TargetManager { get; }

        public CharacterStats BaseStats { get; protected set; }
        public CharacterStats FinalStats { get; protected set; }

        public HitboxController HitboxController => _hitboxController;
        public HurtboxController HurtboxController => _hurtboxController;

        // temp
        public HackPlayerConfig HackConfig;

        private void Awake() {
            InitializeConfig();
            InitializeCharacterComponents();
        }

        private void InitializeConfig() {
            // TODO: implement when we have proper serialized data to controller to UI infrastructure
            UseFakeConfig();
        }

        private void UseFakeConfig() {
            BaseStats = HackConfig.BaseStats;
        }

        private void InitializeCharacterComponents() {

        }
    }
}
