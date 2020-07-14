using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class NPCTargetManager : CharacterComponent {

        private const int TargetsInRangeMaximum = 10;
        private const float DetectionFactor = .2f;
        public const float AttentionBuffer = 3f;

        // current focused target
        private HostileEntry _currentTarget = new HostileEntry();
        public ICharacter CurrentTarget => _currentTarget.Hostile;

        [SerializeField] private float _visionDistance;
        [SerializeField] private float _visionRange;
        [SerializeField] private Transform _rayOrigin;
        [SerializeField] private LayerMask _charactersLayers;
        [SerializeField] private LayerMask _scanLayers;
        [SerializeField] private CharacterTag _hostilesTags;
        
        // current list of registered hostiles
        public readonly List<HostileEntry> RegisteredHostiles = new List<HostileEntry>();
        private List<HostileEntry> _registeredHostilesToBeRemoved = new List<HostileEntry>();
        private Vector3 _bodyPosition => _character.MoveController.Body.position;
        private Collider[] _targetsWithinRange = new Collider[TargetsInRangeMaximum];

        public event Action<HostileEntry> OnNewHostileRegistered;
        public event Action<int> OnRegisteredHostilesUpdated;
        public event Action OnCurrentTargetSet;

        public void OverrideCurrentTarget(ICharacter character) {
            _currentTarget = new HostileEntry() {
                Hostile = character,
                DetectionValue = 1 + AttentionBuffer,
                Detected = true
            };
            OnCurrentTargetSet?.Invoke();
        }

        public void ClearCurrentTarget() {
            _currentTarget.Hostile = null;
            OnCurrentTargetSet?.Invoke();
        }

        // registers or updates a hostile in the list
        public void RegisterOrUpdateHostile(ICharacter character, float startingDetection = 0f) {
            int index = RegisteredHostiles.FindIndex(x => x.Hostile == character);
            // if this one has been found before, update its state
            if (index >= 0) {
                RegisteredHostiles[index].DetectionValue = ChangeDetectionValue(RegisteredHostiles[index].DetectionValue, true);
                RegisteredHostiles[index].Detected = true;
                return;
            }
            // add this as a new hostile
            HostileEntry newEntry = new HostileEntry() {
                Hostile = character,
                DetectionValue = startingDetection,
                Detected = true
            };
            RegisteredHostiles.Add(newEntry);
            OnNewHostileRegistered?.Invoke(newEntry);
            if(newEntry.DetectionValue >= 1f && CurrentTarget == null) {
                OverrideCurrentTarget(character);
            }
        }

        // scans the surrounding area and updates list of aware hostiles
        public void ScanForHostiles() {
            if(CurrentTarget != null) {
                ScanForCurrentTarget();
                return;
            }
            // get all relevant colliders within a certain radius
            int count = Physics.OverlapSphereNonAlloc(_bodyPosition, _visionDistance, _targetsWithinRange, _charactersLayers, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++) {
                ICharacter otherCharacter = _targetsWithinRange[i].GetComponent<ICharacter>();
                // ignore if this does not have a character component or is the NPC character
                if (otherCharacter == null || otherCharacter == _character) {
                    continue;
                }
                // ignore if this does not have any hostile character tags
                if (!_hostilesTags.HasFlag(otherCharacter.CharacterStatManager.CharacterTags)) {
                    continue;
                }
                // if this target is visible, register/update this target
                if (ScanForTarget(otherCharacter)) {
                    RegisterOrUpdateHostile(otherCharacter);
                }
            }
            UpdateRegisteredHostiles();
        }

        private void UpdateRegisteredHostiles() {
            for(int i = 0; i < RegisteredHostiles.Count; i++) {
                HostileEntry entry = RegisteredHostiles[i];
                // decrement the state of those that were not found
                if (!entry.Detected) {
                    entry.DetectionValue = ChangeDetectionValue(entry.DetectionValue, false);
                    // mark for removal if the detection value is 0
                    if(entry.DetectionValue < 0f) {
                        _registeredHostilesToBeRemoved.Add(entry);
                    }
                }
                // Set as current target if threshold has been crossed
                if(entry.DetectionValue >= 1f) {
                    OverrideCurrentTarget(entry.Hostile);
                }
                // reset the detection state off all registered hostiles
                entry.Detected = false;
            }
            // remove all hostiles that have remained out of sight for too long
            for(int i = 0; i < _registeredHostilesToBeRemoved.Count; i++) {
                RegisteredHostiles.Remove(_registeredHostilesToBeRemoved[i]);
            }
            _registeredHostilesToBeRemoved.Clear();
            OnRegisteredHostilesUpdated?.Invoke(RegisteredHostiles.Count);
        }

        private void ScanForCurrentTarget() {
            if(_currentTarget.DetectionValue <= 0f) {
                ClearCurrentTarget();
                return;
            }
            _currentTarget.Detected = ScanForTarget(CurrentTarget);
            _currentTarget.DetectionValue = ChangeDetectionValue(_currentTarget.DetectionValue, _currentTarget.Detected);
            _currentTarget.DetectionValue = Mathf.Clamp(_currentTarget.DetectionValue, 0f, 1 + AttentionBuffer);
        }

        private static float ChangeDetectionValue(float baseValue, bool increment) {
            return increment ? baseValue + Time.deltaTime * DetectionFactor :
                baseValue - Time.deltaTime * DetectionFactor;
        }

        public bool ScanForTarget(ICharacter target) {
            if (!TargetWithinRange(target) || !TargetWithinFieldOfView(target)) {
                return false;
            }
            return TargetVisible(target);
        }

        public bool TargetVisible(ICharacter target) {
            Vector3 direction = (target.MoveController.Center.position - _rayOrigin.position).normalized;
            if (Physics.Raycast(_rayOrigin.position, direction, out RaycastHit info, _visionDistance, _scanLayers, QueryTriggerInteraction.Ignore)) {
                Collider collider = info.collider;
                ICharacter hitCharacter = collider.GetComponent<ICharacter>();
                if (hitCharacter == target) {
                    return true;
                }
            }
            return false;
        }

        public bool TargetWithinRange(ICharacter target) {
            float distance = Vector3.Distance(target.MoveController.Center.position, _rayOrigin.position);
            return distance <= _visionDistance;
        }

        private bool TargetWithinFieldOfView(ICharacter target) {
            Vector3 direction = (target.MoveController.Center.position - _rayOrigin.position).normalized;
            float angle = Vector3.Angle(_rayOrigin.forward, direction);
            return angle <= _visionRange;
        }
    }

    public class HostileEntry {
        public ICharacter Hostile;
        public float DetectionValue;
        public bool Detected;
    }
}
