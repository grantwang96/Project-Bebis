using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerCharacter : MonoBehaviour, ICharacter {

        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private HitboxController _hitboxController;
        [SerializeField] private EquipmentManager _equipmentManager;

        private PlayerDamageable _playerDamageable;
        private PlayerMoveController _playerMoveController;
        private PlayerActionController _playerActionController;
        private PlayerAnimationController _playerAnimationController;
        private PlayerCharacterStatManager _playerCharacterStatManager;

        public IDamageable Damageable => _playerDamageable;
        public IMoveController MoveController => _playerMoveController;
        public IActionController ActionController => _playerActionController;
        public IAnimationController AnimationController => _playerAnimationController;
        public ICharacterStatManager CharacterStatManager => _playerCharacterStatManager;

        public CharacterStats BaseStats { get; private set; }
        public CharacterStats FinalStats { get; private set; }

        public HitboxController HitboxController => _hitboxController;

        public event Action OnAwake;
        public event Action OnStart;
        public event Action OnUpdate;
        public event Action OnFixedUpdate;

        public event Action<ActionStatus> OnActionStatusUpdated;

        // temp
        public HackPlayerConfig HackConfig;
        [SerializeField] private ActionStatus _status;

        private void Awake() {
            InitializeConfig();
            InitializeCharacterComponents();
            OnAwake?.Invoke();
        }

        private void InitializeConfig() {
            // TODO: implement when we have proper serialized data to controller to UI infrastructure
            UseFakeConfig();
        }

        private void UseFakeConfig() {
            BaseStats = HackConfig.BaseStats;
        }

        private void InitializeCharacterComponents() {
            _playerDamageable = new PlayerDamageable(this);
            _playerMoveController = new PlayerMoveController(this, _rigidbody);
            _playerActionController = new PlayerActionController(this);
            _playerAnimationController = new PlayerAnimationController(this, _animator, _equipmentManager);
            _playerCharacterStatManager = new PlayerCharacterStatManager(this, _equipmentManager);
        }

        // Start is called before the first frame update
        void Start() {
            OnStart?.Invoke();
        }

        // Update is called once per frame
        void Update() {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate() {
            OnFixedUpdate?.Invoke();
        }

        private void UpdateActionStatus(ActionStatus status) {
            _status = status;
            OnActionStatusUpdated?.Invoke(status);
        }
    }
}
