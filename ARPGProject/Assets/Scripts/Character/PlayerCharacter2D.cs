using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacter2D : PlayerCharacter {

        [SerializeField] private PlayerDamageable _playerDamageable;
        [SerializeField] private PlayerMoveControllerV2 _playerMoveController;
        [SerializeField] private PlayerActionController _playerActionController;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private PlayerAnimationController2D _playerAnimationController;
        [SerializeField] private PlayerCharacterStatManager _playerCharacterStatManager;

        public override IDamageable Damageable => _playerDamageable;
        public override IMoveController MoveController => _playerMoveController;
        public override IActionController ActionController => _playerActionController;
        public override IAnimationController AnimationController => _playerAnimationController;
        public override ICharacterStatManager CharacterStatManager => _playerCharacterStatManager;

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
