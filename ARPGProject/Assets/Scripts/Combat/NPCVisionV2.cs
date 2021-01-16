using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class NPCVisionV2 : IDetectableScanner
    {
        private ICharacterV2 _character;
        private readonly List<IDetectable> _foundDetectables = new List<IDetectable>();
        private float _angleRange;
        private LayerMask _layerMask;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            _layerMask = MonoBehaviourMetaData.Instance.NPCVisionLayerMask;
            _angleRange = 80f; // TODO: have this be data driven
        }

        public bool ScanForDetectable(IDetectable detectable) {
            return Raycast(detectable.Transform, _angleRange, _layerMask);
        }

        public IReadOnlyList<IDetectable> ScanForDetectables(float radius) {
            _foundDetectables.Clear();
            Collider[] collidersInRange = Physics.OverlapSphere(_character.Head.position, radius, _layerMask);
            for(int i = 0; i < collidersInRange.Length; i++) {
                if (Raycast(collidersInRange[i], _angleRange, _layerMask, out IDetectable detectable)) {
                    _foundDetectables.Add(detectable);
                }
            }
            return _foundDetectables;
        }

        private bool Raycast(Collider collider, float angleRange, LayerMask layerMask, out IDetectable detectable) {
            detectable = null;
            float distance = Vector3.Distance(_character.Head.position, collider.transform.position);
            // check if object is within vision cone
            Vector3 direction = (collider.transform.position - _character.Head.position).normalized;
            float angle = Vector3.Angle(_character.Head.forward, direction);
            if(angle > angleRange) {
                return false;
            }
            // check if this object is a detectable
            detectable = collider.GetComponent<IDetectable>();
            if(detectable == null) {
                return false;
            }
            // perform the raycast
            if (Physics.Raycast(_character.Head.position, direction, out RaycastHit hitInfo, distance, layerMask)) {
                if (hitInfo.transform == detectable.Transform) {
                    return true;
                }
            }
            return false;
        }

        private bool Raycast(Transform detectableTransform, float angleRange, LayerMask layerMask) {
            float distance = Vector3.Distance(_character.Head.position, detectableTransform.position);
            // check if object is within vision cone
            Vector3 direction = (detectableTransform.position - _character.Head.position).normalized;
            float angle = Vector3.Angle(_character.Head.forward, direction);
            // perform the raycast
            if (Physics.Raycast(_character.Head.position, direction, out RaycastHit hitInfo, distance, layerMask)) {
                if (hitInfo.transform == detectableTransform) {
                    return true;
                }
            }
            return false;
        }
    }
}
