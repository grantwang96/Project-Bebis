using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummy : MonoBehaviour {

        private IDamageable _damageable;
        private IMoveController _moveController;

        [SerializeField] private Hurtbox _hurtBox;
        [SerializeField] private Rigidbody2D _rigidbody;

        private void Awake() {
            _moveController = new TestDummyMoveController(_rigidbody);
            _damageable = new TestDummyDamageable(_hurtBox, _moveController);
        }
    }
}
