using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Bebis {
    public class NPCNavigator : CharacterComponent {

        private const int RandomLocSearchSegments = 8;

        public Vector3 MoveInput;
        public Vector3 RotationInput;
        public Vector3 TargetPosition;

        [SerializeField] private float _navMeshRadius;

        private NavMeshPath _path;
        private Vector3 _characterPosition => _character.MoveController.Body.position;

        private List<Vector3> _radiusDirections = new List<Vector3>();

        protected override void Awake() {
            base.Awake();
            _path = new NavMeshPath();
            BuildRadiusDirections();
        }

        private void BuildRadiusDirections() {
            for (int i = 0; i < RandomLocSearchSegments; i++) {
                float degrees = 360f * ((float)i / RandomLocSearchSegments);
                Vector3 newDir = new Vector3(Mathf.Cos(degrees), 0f, Mathf.Sin(degrees));
                _radiusDirections.Add(newDir);
            }
        }

        public NavMeshPath CalculatePath(Vector3 target) {
            if(NavMesh.CalculatePath(_characterPosition, target, NavMesh.AllAreas, _path)) {
                return _path;
            }
            return null;
        }

        public bool CanDirectPathToPoint(Vector3 point) {
            return NavMesh.Raycast(_characterPosition, point, out NavMeshHit hit, NavMesh.AllAreas);
        }

        private Vector3 GetNavMeshPosition() {
            if(NavMesh.SamplePosition(_characterPosition, out NavMeshHit hit, _navMeshRadius, NavMesh.AllAreas)) {
                return hit.position;
            }
            return _characterPosition;
        }

        public Vector3 GetRandomLocationAtDistanceRadius(float radius) {
            List<Vector3> possibleDestinations = new List<Vector3>();
            for(int i = 0; i < _radiusDirections.Count; i++) {
                Vector3 direction = _radiusDirections[i] * radius;
                direction.y = _characterPosition.y;
                if(NavMesh.SamplePosition(_characterPosition + direction, out NavMeshHit hit, _navMeshRadius, NavMesh.AllAreas) ||
                   NavMesh.CalculatePath(GetNavMeshPosition(), hit.position, NavMesh.AllAreas, new NavMeshPath())) {
                    possibleDestinations.Add(hit.position);
                }
            }
            return possibleDestinations.Count > 0 ? possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)] : transform.position;
        }
    }
}
