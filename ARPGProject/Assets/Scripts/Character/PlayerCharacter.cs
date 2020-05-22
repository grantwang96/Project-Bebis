using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacter : MonoBehaviour, ICharacter {

        [SerializeField] private PlayerDamageable _playerDamageable;
        [SerializeField] private PlayerMoveControllerV2 _playerMoveController;
        [SerializeField] private PlayerActionController _playerActionController;
        [SerializeField] private HitboxController _hitboxController;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private PlayerAnimationController2D _playerAnimationController;
        [SerializeField] private PlayerCharacterStatManager _playerCharacterStatManager;

        public IDamageable Damageable => _playerDamageable;
        public IMoveController MoveController => _playerMoveController;
        public IActionController ActionController => _playerActionController;
        public IAnimationController AnimationController => _playerAnimationController;
        public ICharacterStatManager CharacterStatManager => _playerCharacterStatManager;

        public CharacterStats BaseStats { get; private set; }
        public CharacterStats FinalStats { get; private set; }

        public HitboxController HitboxController => _hitboxController;

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
