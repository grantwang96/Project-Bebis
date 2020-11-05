using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis {
    public class NPCCharacter : MonoBehaviour, ICharacter {

        [SerializeField] private NPCDamageable _damageable;
        [SerializeField] private NPCMoveController3D _moveController;
        [SerializeField] private NPCActionController _actionController;
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private NPCAnimationController3D _animationController;
        [SerializeField] private NPCCharacterStatManager _npcCharacterStatManager;
        [SerializeField] private NPCTargetManager _npcTargetManager;

        [SerializeField] protected HitboxController _hitboxController;
        [SerializeField] protected HurtboxController _hurtboxController;
        [SerializeField] private NPCMoveInfoProvider _npcMoveInfoProvider;
        [SerializeField] private NPCActionInfoProvider _npcActionInfoProvider;

        public string UniqueId { get; private set; }

        public IDamageable Damageable => _damageable;
        public IMoveController MoveController => _moveController;
        public IActionController ActionController => _actionController;
        public IAnimationController AnimationController => _animationController;
        public ICharacterStatManager CharacterStatManager => _npcCharacterStatManager;
        public ITargetManager TargetManager => _npcTargetManager;
        public HitboxController HitboxController => _hitboxController;
        public HurtboxController HurtboxController => _hurtboxController;
        public IMoveControllerInfoProvider MoveInfoProvider => _npcMoveInfoProvider;
        public IActionControllerInfoProvider ActionInfoProvider => _npcActionInfoProvider;

        public CharacterStats BaseStats { get; protected set; }
        public CharacterStats FinalStats { get; protected set; }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        public IPooledObject Spawn() {
            return Instantiate(this, PooledObjectRoot.Instance.transform);
        }

        public void Despawn() {
            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

        public void Initialize(PooledObjectInitializationData initializationData) {
            NPCCharacterInitData initData = initializationData as NPCCharacterInitData;
            if (initData == null) {
                return;
            }
            UniqueId = initData.UniqueId;
            MoveController.Body.position = initData.SpawnLocation;
        }

        private void Awake() {
            InitializeCharacterComponents();
        }

        private void InitializeCharacterComponents() {

        }
    }

    public class NPCCharacterInitData : CharacterInitializationData
    {

    }
}
