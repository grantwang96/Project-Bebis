using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Bebis {
    public class NPCNavigator : CharacterComponent {

        private const int RandomLocSearchSegments = 8;

        [SerializeField] private NavMeshAgent _navMeshAgent;

        public Vector3 MoveInput;
        public Vector3 RotationInput;
        public Vector3 TargetPosition;

        public event Action<NavMeshPath> OnCalculatePathCompleted;

        private NavMeshPath _path;
        private Coroutine _pathCalculationProcess;
        private Vector3 _characterPosition => _character.MoveController.Body.position;

        private List<Vector3> _radiusDirections = new List<Vector3>();

        protected override void Awake() {
            base.Awake();
            _path = new NavMeshPath();
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            BuildRadiusDirections();
        }

        private void BuildRadiusDirections() {
            for (int i = 0; i < RandomLocSearchSegments; i++) {
                float degrees = 360f * ((float)i / RandomLocSearchSegments);
                Vector3 newDir = new Vector3(Mathf.Cos(degrees), 0f, Mathf.Sin(degrees));
                _radiusDirections.Add(newDir);
            }
        }

        public void CalculatePath(Vector3 target) {
            _navMeshAgent.nextPosition = _characterPosition;
            if (_navMeshAgent.pathPending) {
                StopCoroutine(_pathCalculationProcess);
            }
            _pathCalculationProcess = StartCoroutine(ProcessPathCalculation(target));
        }

        public bool CanDirectPathToPoint(Vector3 point) {
            return NavMesh.Raycast(_navMeshAgent.nextPosition, point, out NavMeshHit hit, NavMesh.AllAreas);
        }

        public Vector3 GetRandomLocationAtDistanceRadius(float radius) {
            List<Vector3> possibleDestinations = new List<Vector3>();
            for(int i = 0; i < _radiusDirections.Count; i++) {
                Vector3 direction = _radiusDirections[i] * radius;
                direction.y = _characterPosition.y;
                if(NavMesh.SamplePosition(_characterPosition + direction, out NavMeshHit hit, _navMeshAgent.radius * 2f, NavMesh.AllAreas)) {
                    possibleDestinations.Add(hit.position);
                }
            }
            return possibleDestinations.Count > 0 ? possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)] : transform.position;
        }

        private IEnumerator ProcessPathCalculation(Vector3 target) {
            _navMeshAgent.CalculatePath(target, _path);
            while (_navMeshAgent.pathPending) {
                yield return new WaitForFixedUpdate();
            }
            OnCalculatePathCompleted?.Invoke(_path);
        }
    }
}
