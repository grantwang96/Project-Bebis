using UnityEngine;

namespace Bebis {
    public interface IMoveController {
        int XPos { get; }
        int YPos { get; }

        Vector3 WorldPos { get; }
        Vector3 Move { get; }
        Vector3 Rotation { get; }

        void AddForce(Vector3 direction, float force, bool overrideForce = false);
    }
}
