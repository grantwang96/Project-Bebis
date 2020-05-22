using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummy : MonoBehaviour {

        [SerializeField] private GameObject _rootObj;
        private IDamageable _damageable;
        private IMoveController _moveController;

        [SerializeField] private Hurtbox _hurtBox;
        [SerializeField] private Rigidbody2D _rigidbody;

        private void Awake() {
            _damageable = _rootObj.GetComponent<IDamageable>();
            _moveController = _rootObj.GetComponent<IMoveController>();
        }
    }
}
