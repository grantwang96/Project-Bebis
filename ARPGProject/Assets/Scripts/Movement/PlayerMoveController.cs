using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerMoveController : PlayerCharacterComponent, IMoveController {

        private const float InputMoveThreshold = 1f;

        public int XPos { get; private set; }
        public int YPos { get; private set; }

        public Vector3 WorldPos => _rigidbody2D.position;
        public Vector3 Move { get; private set; }
        public Vector3 Rotation { get; private set; } = new Vector3(0f, -1f);

        private Rigidbody2D _rigidbody2D;

        private float _moveSpeed = 5f;
        private float _acceleration = 5f;
        private Vector2 _currentMove;

        private bool _canMove = true;
        private bool _canRotate = true;

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            if (overrideForce) {
                _rigidbody2D.velocity = Vector2.zero;
            }
            _rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
        }

        public PlayerMoveController(PlayerCharacter character, Rigidbody2D rigidbody) : base(character) {
            _rigidbody2D = rigidbody;
            SubscribeToEvents();
        }

        private void SubscribeToEvents() {
            _character.OnFixedUpdate += OnFixedUpdate;
        }

        private void UnsubscribeToEvents() {
            _character.OnFixedUpdate -= OnFixedUpdate;
        }

        private void OnFixedUpdate() {
            ProcessMovement(InputController.Instance.MoveInput);
            ProcessRotation(InputController.Instance.MoveInput);
        }

        private void ProcessMovement(Vector2 moveInput) {
            if (!_canMove) {
                return;
            }
            if (!_character.ActionController.Permissions.HasFlag(ActionPermissions.Movement)) {
                return;
            }
            Move = moveInput;
            _currentMove = moveInput * _moveSpeed;
            if (_rigidbody2D.velocity.magnitude < _moveSpeed) {
                _rigidbody2D.velocity = (moveInput * _moveSpeed);
            }
        }

        private void ProcessRotation(Vector2 moveInput) {
            if (!_canRotate) {
                return;
            }
            if (!_character.ActionController.Permissions.HasFlag(ActionPermissions.Rotation)) {
                return;
            }
            if (Mathf.Approximately(moveInput.x, 0f) && Mathf.Approximately(moveInput.y, 0f)) {
                return;
            }
            Rotation = moveInput;
        }
    }
}
