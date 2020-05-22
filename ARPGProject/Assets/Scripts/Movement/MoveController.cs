using UnityEngine;

namespace Bebis {
    public interface IMoveController {
        Vector3 Move { get; }
        Vector3 Rotation { get; }
        bool CanJump { get; }
        Transform Body { get; }

        void AddForce(Vector3 direction, float force, bool overrideForce = false);
    }
}
