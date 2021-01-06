using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public abstract class MoveControllerState
    {
        public Vector3 ExternalForces => _externalForces;

        protected CharacterController _characterController;
        protected ICharacterV2 _character;
        protected IMoveControllerStateMachine _stateMachine;
        protected IMoveControllerV2 _moveController => _character.MoveController;

        protected Vector3 _externalForces;
        protected Vector3 _movementInput;
        protected float _movementInputSpeed;

        public MoveControllerState(CharacterController characterController, ICharacterV2 character, IMoveControllerStateMachine stateMachine) {
            _characterController = characterController;
            _character = character;
            _stateMachine = stateMachine;
        }

        public void ApplyForce(Vector3 force, bool overrideForce = false) {
            if (overrideForce) {
                _externalForces = Vector3.zero;
            }
            _externalForces += force;
        }

        public virtual void Enter(Vector3 externalForces) {
            _externalForces = externalForces;
        }
        public virtual void Execute() {
            ProcessGravity();
            ReadMoveInput();
            ProcessMovement();
            ProcessRotation();
        }

        protected void ProcessGravity() {
            if (_externalForces.y >= Physics.gravity.y) {
                _externalForces.y += Physics.gravity.y * Time.deltaTime;
            }
        }

        protected virtual void ReadMoveInput() {
            _movementInput = _character.UnitController.MovementInput;
            _movementInput *= _movementInputSpeed;
        }

        protected virtual void ProcessMovement() {
            Vector3 totalVelocity = _externalForces;
            if (!_moveController.MovementRestrictions.Restricted) {
                // TODO: handle movement input when aerial vs. grounded
                totalVelocity += _movementInput;
            }
            _characterController.Move(totalVelocity * Time.deltaTime);
        }

        protected virtual void ProcessRotation() {
            if (!_moveController.LookRestrictions.Restricted && _character.UnitController.RotationInput.magnitude > 0f) {
                Transform transform = _characterController.transform;
                transform.forward = Vector3.RotateTowards(
                    transform.forward, _character.UnitController.RotationInput, _moveController.Data.TurnSpeed * Time.deltaTime, 0f);
            }
        }
    }

    public class GroundedMoveControllerState : MoveControllerState
    {
        public GroundedMoveControllerState(CharacterController characterController, ICharacterV2 character, IMoveControllerStateMachine stateMachine) :
            base(characterController, character, stateMachine) {
            _movementInputSpeed = character.MoveController.Data.GroundSpeed;
        }

        public override void Enter(Vector3 externalForces) {
            base.Enter(externalForces);
            if (!_moveController.MovementRestrictions.Restricted) {
                _externalForces.x = 0f;
                _externalForces.z = 0f;
            }
            // Debug.Log($"Entered {nameof(GroundedMoveControllerState)}");
        }

        public override void Execute() {
            base.Execute();
            ProcessGroundPhysics();
            if (!_characterController.isGrounded) {
                _stateMachine.ChangeState(MoveControllerStateId.Aerial);
            }
        }

        private void ProcessGroundPhysics() {
            if (!_moveController.MovementRestrictions.Restricted) {
                Vector3 lateralExternalForces = new Vector3(_externalForces.x, 0f, _externalForces.z);
                lateralExternalForces = Vector3.MoveTowards(lateralExternalForces, Vector3.zero, _moveController.Data.GroundDrag * Time.deltaTime);
                _externalForces.x = lateralExternalForces.x;
                _externalForces.z = lateralExternalForces.z;
            }
        }
    }

    public class AerialMoveContollerState : MoveControllerState
    {
        public AerialMoveContollerState(CharacterController characterController, ICharacterV2 character, IMoveControllerStateMachine stateMachine) : 
            base(characterController, character, stateMachine) {
            _movementInputSpeed = character.MoveController.Data.AirSpeed;
        }

        public override void Enter(Vector3 externalForces) {
            base.Enter(externalForces);
            // Debug.Log($"Entered {nameof(AerialMoveContollerState)}");
        }

        public override void Execute() {
            base.Execute();
            ProcessAirPhysics();
            if (_characterController.isGrounded) {
                _stateMachine.ChangeState(MoveControllerStateId.Grounded);
            }
        }

        private void ProcessAirPhysics() {
            if (!_moveController.MovementRestrictions.Restricted) {
                Vector3 lateralExternalForces = new Vector3(_externalForces.x, 0f, _externalForces.z);
                lateralExternalForces = Vector3.MoveTowards(lateralExternalForces, Vector3.zero, _moveController.Data.AirDrag * Time.deltaTime);
                _externalForces.x = lateralExternalForces.x;
                _externalForces.z = lateralExternalForces.z;
            }
        }

        protected override void ReadMoveInput() {
            base.ReadMoveInput();
            Vector3 horizontalForces = new Vector3(_externalForces.x, 0f, _externalForces.z);
            Vector3 previewedVelocity = horizontalForces + _movementInput;
            float previewedMagnitude = previewedVelocity.magnitude;
            bool canAddMoveInput = previewedMagnitude <= _moveController.Data.MaxAirSpeed;
            if (!canAddMoveInput) {
                _movementInput = Vector3.zero;
            }
        }

        protected override void ProcessRotation() {
            // disallow rotation inputs while in the air (actions can override character rotation, however)
        }
    }
}
