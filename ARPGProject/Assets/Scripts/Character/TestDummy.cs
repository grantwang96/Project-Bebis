using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis {
    public class TestDummy : MonoBehaviour, ICharacter {

        [SerializeField] private GameObject _rootObj;
        
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private HitboxController _hitboxController;
        [SerializeField] private HurtboxController _hurtboxController;
        [SerializeField] private NPCTargetManager _npcTargetManager;

        public IDamageable Damageable { get; private set; }
        public IMoveController MoveController { get; private set; }
        public IActionController ActionController { get; private set; }
        public IAnimationController AnimationController { get; private set; }
        public ICharacterStatManager CharacterStatManager { get; private set; }
        public ITargetManager TargetManager => _npcTargetManager;
        public HitboxController HitboxController => _hitboxController;
        public HurtboxController HurtboxController => _hurtboxController;
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

        private void Awake() {
            Damageable = _rootObj.GetComponent<IDamageable>();
            MoveController = _rootObj.GetComponent<IMoveController>();
            ActionController = _rootObj.GetComponent<IActionController>();

            _hitboxController = _rootObj.GetComponent<HitboxController>();
            _hurtboxController = _rootObj.GetComponent<HurtboxController>();
        }
    }
}
