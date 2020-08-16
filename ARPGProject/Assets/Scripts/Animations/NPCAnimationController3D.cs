using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCAnimationController3D : GroundedCharacterAnimationController {

        [SerializeField] private GameObject _characterGO;
        private ICharacter _character;

        private void Awake() {
            _character = _characterGO.GetComponent<ICharacter>();
        }

        protected override void ProcessMove() {
            if (!_enabled) {
                return;
            }
            float moveValue = _character.MoveController.MoveMagnitude;
            _animator.SetFloat(MoveKey, moveValue);
        }

        protected override void ProcessAirState() {
            _animator.SetBool(AirborneKey, !_character.MoveController.IsGrounded);
        }
    }
}
