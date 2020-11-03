using UnityEngine;

namespace Bebis {

    public enum CharacterMoveMode {
        Stopped, Walk, Run
    }

    public interface IMoveController {
        float MoveMagnitude { get; }
        Vector3 Move { get; }
        Vector3 Rotation { get; }
        bool IsGrounded { get; }
        Transform Body { get; }
        Transform Center { get; }
        RestrictionController MoveRestrictions { get; }
        RestrictionController LookRestrictions { get; }

        float Height { get; }

        void AddForce(Vector3 direction, float force, bool overrideForce = false);
        void AddForce(Vector3 totalForce, bool overrideForce = false);
        void OverrideMovement(Vector3 direction);
        void OverrideRotation(Vector3 direction);
    }

    public interface IMoveControllerInfoProvider
    {
        Vector3 IntendedMoveDirection { get; }
        Vector3 IntendedLookDirection { get; }
    }
}
