using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class CharacterAnimationController : GroundedCharacterAnimationController, ICharacterComponent
    {
        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
        }

        protected override void ProcessAirState() {
            _animator.SetBool(AirborneKey, !_character.MoveController.IsGrounded);
        }

        protected override void ProcessMove() {
            if (!_enabled) {
                return;
            }
            float moveValue = _character.UnitController.MovementInput.magnitude;
            moveValue = Mathf.Clamp01(moveValue);
            _animator.SetFloat(MoveKey, moveValue);
        }
    }
}
