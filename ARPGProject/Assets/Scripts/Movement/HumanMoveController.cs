using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HumanMoveController : MonoBehaviour, IMoveControllerV2, IMoveControllerStateMachine
    {
        public RestrictionController MovementRestrictions => _movementRestrictions;
        public RestrictionController LookRestrictions => _lookRestrictions;
        public bool IsGrounded => _currentMoveControllerState == _groundedState;
        public MoveControllerData Data => _moveControllerData;

        public event Action<bool> OnIsGroundedUpdated;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private MoveControllerData _moveControllerData;
        [SerializeField] private RestrictionController _movementRestrictions;
        [SerializeField] private RestrictionController _lookRestrictions;

        private ICharacterV2 _character;

        private MoveControllerState _currentMoveControllerState;
        private GroundedMoveControllerState _groundedState;
        private AerialMoveContollerState _aerialState;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            MovementRestrictions.Clear();
            LookRestrictions.Clear();
            MovementRestrictions.OnRestrictionUpdated += OnMoveRestrictionsUpdated;
            LookRestrictions.OnRestrictionUpdated += OnLookRestrictionsUpdated;
            _groundedState = new GroundedMoveControllerState(_characterController, _character, this);
            _aerialState = new AerialMoveContollerState(_characterController, _character, this);
            _currentMoveControllerState = _groundedState;
            _currentMoveControllerState.Enter(Physics.gravity);
        }

        public void AddForce(Vector3 direction, float force, bool overrideForce = false) {
            AddForce(direction * force, overrideForce);
        }

        public void AddForce(Vector3 velocity, bool overrideForce = false) {
            _currentMoveControllerState.ApplyForce(velocity, overrideForce);
        }

        public void OverrideMovement(Vector3 movement) {
            
        }

        public void OverrideRotation(Vector3 direction) {
            _character.GameObject.transform.forward = direction;
        }

        public void ChangeState(MoveControllerStateId stateId) {
            Vector3 previousExternalForces = _currentMoveControllerState.ExternalForces;
            switch (stateId) {
                case MoveControllerStateId.Grounded:
                    _currentMoveControllerState = _groundedState;
                    OnIsGroundedUpdated?.Invoke(true);
                    break;
                case MoveControllerStateId.Aerial:
                    _currentMoveControllerState = _aerialState;
                    OnIsGroundedUpdated?.Invoke(false);
                    break;
                default:
                    break;
            }
            _currentMoveControllerState.Enter(previousExternalForces);
        }

        private void FixedUpdate() {
            ProcessCurrentState();
        }

        private void ProcessCurrentState() {
            _currentMoveControllerState?.Execute();
        }

        private void OnMoveRestrictionsUpdated() {

        }

        private void OnLookRestrictionsUpdated() {

        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (!MovementRestrictions.Restricted) {
                
            }
        }
    }
}
