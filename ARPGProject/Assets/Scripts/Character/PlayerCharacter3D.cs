using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerCharacter3D : PlayerCharacter {

        [SerializeField] private PlayerDamageable _playerDamageable;
        [SerializeField] private PlayerMoveController _playerMoveController;
        [SerializeField] private PlayerActionController _playerActionController;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private PlayerAnimationController _playerAnimationController;
        [SerializeField] private PlayerCharacterStatManager _playerCharacterStatManager;
        [SerializeField] private PlayerTargetManager _playerTargetManager;

        public override IDamageable Damageable => _playerDamageable;
        public override IMoveController MoveController => _playerMoveController;
        public override IActionController ActionController => _playerActionController;
        public override IAnimationController AnimationController => _playerAnimationController;
        public override ICharacterStatManager CharacterStatManager => _playerCharacterStatManager;
        public override ITargetManager TargetManager => _playerTargetManager;

        private void Awake() {
            Instance = this;
            InitializeConfig();
            InitializeCharacterComponents();
        }

        private void Start() {
            _playerMoveController.Initialize(_playerController);
            _playerActionController.Initialize(_playerController);
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
