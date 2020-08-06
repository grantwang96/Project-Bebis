using UnityEngine;

namespace Bebis {
    public interface IMoveController {
        float MoveMagnitude { get; }
        Vector3 Move { get; }
        Vector3 Rotation { get; }
        bool IsGrounded { get; }
        Transform Body { get; }
        Transform Center { get; }

        float Height { get; }

        void AddForce(Vector3 direction, float force, bool overrideForce = false);
        void AddForce(Vector3 totalForce, bool overrideForce = false);
        void OverrideMovement(Vector3 direction);
        void OverrideRotation(Vector3 direction);
    }
}
