using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCNavigator : MonoBehaviour {

        public Vector2 MoveInput { get; set; }
        public Vector2 RotationInput { get; set; }
        public IntVector3 MoveTarget;
    }
}
