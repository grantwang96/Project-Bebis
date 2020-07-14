using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public class WorldSpaceUI : MonoBehaviour {

        // Update is called once per frame
        void Update() {
            transform.forward = (CameraController.Instance.MainCamera.transform.position) - transform.position;
        }
    }
}
