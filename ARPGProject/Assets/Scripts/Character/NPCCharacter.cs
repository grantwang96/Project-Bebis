﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCCharacter : MonoBehaviour, ICharacter {

        [SerializeField] private NPCDamageable _damageable;
        [SerializeField] private NPCMoveController3D _moveController;
        [SerializeField] private NPCActionController _actionController;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private NPCAnimationController3D _animationController;
        [SerializeField] private NPCCharacterStatManager _npcCharacterStatManager;

        [SerializeField] protected HitboxController _hitboxController;
        [SerializeField] protected HurtboxController _hurtboxController;

        public IDamageable Damageable => _damageable;
        public IMoveController MoveController => _moveController;
        public IActionController ActionController => _actionController;
        public IAnimationController AnimationController => _animationController;
        public ICharacterStatManager CharacterStatManager => _npcCharacterStatManager;
        public HitboxController HitboxController => _hitboxController;
        public HurtboxController HurtboxController => _hurtboxController;

        public CharacterStats BaseStats { get; protected set; }
        public CharacterStats FinalStats { get; protected set; }

        private void Awake() {

            InitializeCharacterComponents();
        }

        private void InitializeCharacterComponents() {

        }
    }
}
