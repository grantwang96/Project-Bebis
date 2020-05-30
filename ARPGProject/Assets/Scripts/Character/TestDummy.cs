using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummy : MonoBehaviour, ICharacter {

        [SerializeField] private GameObject _rootObj;
        private IDamageable _damageable;
        private IMoveController _moveController;

        [SerializeField] private Hurtbox _hurtBox;
        [SerializeField] private Rigidbody2D _rigidbody;

        public IDamageable Damageable => _damageable;
        public IMoveController MoveController => _moveController;
        public IActionController ActionController => throw new System.NotImplementedException();
        public IAnimationController AnimationController => throw new System.NotImplementedException();
        public ICharacterStatManager CharacterStatManager => throw new System.NotImplementedException();
        public HitboxController HitboxController => throw new System.NotImplementedException();
        public HurtboxController HurtboxController { get; private set; }

        private void Awake() {
            _damageable = _rootObj.GetComponent<IDamageable>();
            _moveController = _rootObj.GetComponent<IMoveController>();
            HurtboxController = _rootObj.GetComponent<HurtboxController>();
        }
    }
}
