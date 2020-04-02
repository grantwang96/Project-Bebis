using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class ZCorrector : MonoBehaviour {

        [SerializeField] private bool _update;

        private void Start() {
            SetZPosition();
        }

        // Update is called once per frame
        void Update() {
            if (!_update) {
                return;
            }
            SetZPosition();
        }

        private void SetZPosition() {
            Vector3 position = transform.position;
            position.z = Mathf.Round(transform.position.y);
            transform.position = position;
        }
    }
}
