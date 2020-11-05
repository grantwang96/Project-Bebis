using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Winston;

namespace Bebis {
    public abstract class PlayerCharacter : MonoBehaviour, ICharacter {

        public static ICharacter Instance { get; protected set; }

        [SerializeField] protected HitboxController _hitboxController;
        [SerializeField] protected HurtboxController _hurtboxController;
        [SerializeField] protected PlayerController _playerController;

        public string UniqueId { get; private set; }

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
        public IMoveControllerInfoProvider MoveInfoProvider => _playerController;
        public IActionControllerInfoProvider ActionInfoProvider => _playerController;

        // temp
        public HackPlayerConfig HackConfig;

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
            PlayerCharacterInitializationData initData = initializationData as PlayerCharacterInitializationData;
            if(initData == null) {
                return;
            }
            UniqueId = initData.UniqueId;
            MoveController.Body.position = initData.SpawnLocation;
            Instance = this;
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

    public class PlayerCharacterInitializationData : CharacterInitializationData
    {

    }
}
