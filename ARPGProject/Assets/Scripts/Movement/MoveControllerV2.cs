using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IMoveControllerV2 : ICharacterComponent
    {
        RestrictionController MovementRestrictions { get; }
        RestrictionController LookRestrictions { get; }
        bool IsGrounded { get; }
        MoveControllerData Data { get; }

        void AddForce(Vector3 direction, float force, bool overrideForce = false);
        void AddForce(Vector3 force, bool overrideForce = false);

        event Action<bool> OnIsGroundedUpdated;
    }

    public enum MoveControllerStateId
    {
        Grounded,
        Aerial
    }

    public interface IMoveControllerStateMachine
    {
        void ChangeState(MoveControllerStateId stateId);
    }

    [System.Serializable]
    public class MoveControllerData
    {
        [SerializeField] private float _groundSpeed; // hack: replace with data driven
        [SerializeField] private float _airSpeed;
        [SerializeField] private float _maxAirSpeed;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _groundDrag;
        [SerializeField] private float _airDrag;

        public float GroundSpeed => _groundSpeed;
        public float AirSpeed => _airSpeed;
        public float MaxAirSpeed => _maxAirSpeed;
        public float TurnSpeed => _turnSpeed;
        public float GroundDrag => _groundDrag;
        public float AirDrag => _airDrag;
    }
}
