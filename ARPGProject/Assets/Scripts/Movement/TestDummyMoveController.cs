using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestDummyMoveController : IMoveController {

        public int XPos => throw new System.NotImplementedException();
        public int YPos => throw new System.NotImplementedException();

        public Vector3 WorldPos => _rigidbody2D.position;
        public Vector3 Move { get; private set; }
        public Vector3 Rotation { get; private set; }

        private Rigidbody2D _rigidbody2D;

        public TestDummyMoveController(Rigidbody2D rigidbody) {
            _rigidbody2D = rigidbody;
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _rigidbody2D.velocity = Vector2.zero;
            }
            _rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}
