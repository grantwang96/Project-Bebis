using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class NPCTargetManager : CharacterComponent, ITargetManager {

        private const int TargetsInRangeMaximum = 10;
        public const float AttentionBuffer = 3f;

        // current focused target
        private HostileEntry _currentTarget = new HostileEntry();
        public ICharacter CurrentTarget => _currentTarget?.Hostile;

        [SerializeField] private float _visionDistance;
        [SerializeField] private float _visionRange;
        [SerializeField] private Transform _rayOrigin;
        [SerializeField] private LayerMask _charactersLayers;
        [SerializeField] private LayerMask _scanLayers;
        [SerializeField] private CharacterTag _hostilesTags;
        [SerializeField] private float _detectionFactor;

        private readonly Dictionary<ICharacter, HostileEntry> _registeredHostiles = new Dictionary<ICharacter, HostileEntry>();
        private Vector3 _bodyPosition => _character.MoveController.Body.position;
        private Collider[] _targetsWithinRange = new Collider[TargetsInRangeMaximum];

        // current list of registered hostiles
        public IReadOnlyDictionary<ICharacter, HostileEntry> RegisteredHostiles => _registeredHostiles;
        public event Action<HostileEntry> OnNewHostileRegistered;
        public event Action<int> OnRegisteredHostilesUpdated;
        public event Action OnCurrentTargetSet;

        private void SetCurrentTarget(ICharacter character) {
            _currentTarget = _registeredHostiles[character];
            _currentTarget.DetectionValue = 1 + AttentionBuffer;
            OnCurrentTargetSet?.Invoke();
        }

        // override the current target
        public void OverrideCurrentTarget(ICharacter character) {
            if(!_registeredHostiles.TryGetValue(character, out HostileEntry entry)) {
                // add this as a new hostile
                HostileEntry newEntry = new HostileEntry() {
                    Hostile = character,
                    DetectionValue = 1f + AttentionBuffer,
                    Detected = true
                };
                _registeredHostiles.Add(_character, newEntry);
            }
            SetCurrentTarget(character);
        }

        // removes the current target
        public void ClearCurrentTarget() {
            _currentTarget = null;
            OnCurrentTargetSet?.Invoke();
        }

        // registers or updates a hostile in the list
        public void RegisterOrUpdateHostile(ICharacter character, float startingDetection = 0f) {
            // if this one has been found before, update its state
            if (_registeredHostiles.TryGetValue(character, out HostileEntry entry)) {
                entry.DetectionValue = ChangeDetectionValue(entry.DetectionValue, true, _detectionFactor);
                entry.Detected = true;
                return;
            }
            // add this as a new hostile
            HostileEntry newEntry = new HostileEntry() {
                Hostile = character,
                DetectionValue = startingDetection,
                Detected = true
            };
            _registeredHostiles.Add(character, newEntry);
            OnNewHostileRegistered?.Invoke(newEntry);
            if(character == PlayerCharacter.Instance) {
                RegisterPlayer();
            }
            // if this entry has reached threshold level, set as current target
            if(newEntry.DetectionValue >= 1f && CurrentTarget == null) {
                SetCurrentTarget(character);
            }
        }

        private void RegisterPlayer() {
            PlayerTargetsUIManager.Instance.RegisterPlayer(_character);
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

        // run thru all registered hostiles and update detection values. Remove entries that have hit 0
        private void UpdateRegisteredHostiles() {
            List<ICharacter> keys = new List<ICharacter>(_registeredHostiles.Keys);
            for(int i = 0; i < keys.Count; i++) {
                HostileEntry entry = RegisteredHostiles[keys[i]];
                // decrement the state of those that were not found
                if (!entry.Detected) {
                    entry.DetectionValue = ChangeDetectionValue(entry.DetectionValue, false, _detectionFactor);
                    // mark for removal if the detection value is 0
                    if(entry.DetectionValue <= 0f) {
                        _registeredHostiles.Remove(keys[i]);
                    }
                }

                // Set as current target if threshold has been crossed
                if (entry.DetectionValue > 1f) {
                    SetCurrentTarget(entry.Hostile);
                }
                // reset the detection state off all registered hostiles
                entry.Detected = false;
            }
            OnRegisteredHostilesUpdated?.Invoke(RegisteredHostiles.Count);
        }

        private void ScanForCurrentTarget() {
            // clear current target if detection value has returned to normal levels
            if(_currentTarget.DetectionValue < 1f) {
                ClearCurrentTarget();
                return;
            }
            _currentTarget.Detected = ScanForTarget(CurrentTarget);
            _currentTarget.DetectionValue = ChangeDetectionValue(_currentTarget.DetectionValue, _currentTarget.Detected, _detectionFactor);
        }

        private static float ChangeDetectionValue(float baseValue, bool increment, float detectionFactor) {
            return increment ? baseValue + Time.deltaTime * detectionFactor :
                baseValue - Time.deltaTime * detectionFactor;
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
