using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class PlayerAnimationController : GroundedCharacterAnimationController {

        [SerializeField] protected PlayerCharacter _playerCharacter;

        protected override void ProcessMove() {
            if (!_enabled) {
                return;
            }
            float moveValue = _playerCharacter.MoveController.MoveMagnitude;
            _animator.SetFloat(MoveKey, moveValue);
        }

        protected override void ProcessAirState() {
            _animator.SetBool(AirborneKey, !_playerCharacter.MoveController.IsGrounded);
        }
    }
}
