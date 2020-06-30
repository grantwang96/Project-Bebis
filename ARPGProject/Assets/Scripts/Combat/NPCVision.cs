using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class NPCVision : CharacterComponent {

        private const int TargetsInRangeMaximum = 10;

        [SerializeField] private LayerMask _charactersLayers;
        [SerializeField] private LayerMask _scanLayers;
        [SerializeField] private CharacterTag _hostilesTags;
        [SerializeField] private float _visionDistance;
        [SerializeField] private float _visionRange;
        [SerializeField] private Transform _rayOrigin;

        private Vector3 _bodyPosition => _character.MoveController.Body.position;
        private Collider[] _targetsWithinRange = new Collider[TargetsInRangeMaximum];

        public void ScanForHostiles(List<ICharacter> hostileCharacters) {
            hostileCharacters.Clear();
            int count = Physics.OverlapSphereNonAlloc(_bodyPosition, _visionDistance, _targetsWithinRange, _charactersLayers, QueryTriggerInteraction.Collide);
            for(int i = 0; i < count; i++) {
                ICharacter otherCharacter = _targetsWithinRange[i].GetComponent<ICharacter>();
                if(otherCharacter == null || otherCharacter == _character) {
                    continue;
                }
                if (_hostilesTags.HasFlag(otherCharacter.CharacterStatManager.CharacterTags)) {
                    hostileCharacters.Add(otherCharacter);
                }
            }
        }

        public bool ScanForTarget(ICharacter character) {
            Vector3 direction = (character.MoveController.Body.position - _rayOrigin.position).normalized;
            float angle = Vector3.Angle(_rayOrigin.forward, direction);
            if(angle > _visionRange) {
                return false;
            }
            if(Physics.Raycast(_rayOrigin.position, direction, out RaycastHit info, _visionDistance, _scanLayers, QueryTriggerInteraction.Ignore)) {
                Collider collider = info.collider;
                ICharacter hitCharacter = collider.GetComponent<ICharacter>();
                if(hitCharacter == character) {
                    return true;
                }
            }
            return false;
        }
    }
}
