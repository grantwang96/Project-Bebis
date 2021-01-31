using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class NPCDetectableManager : IDetectableManager
    {
        public const float DetectedThreshold = 5f;

        // event to inform that an object has been registered
        public event Action<IDetectable> OnDetectedObjectRegistered;
        // event to inform tha an object has been deregistered
        public event Action<IDetectable> OnDetectedObjectDeregistered;
        // event to inform that an object has been detected
        public event Action<IDetectable> OnDetectedObject;

        // collection of detectable objects (characters, dead bodies, etc.)
        private readonly Dictionary<IDetectable, DetectableEntry> _detectedObjects = new Dictionary<IDetectable, DetectableEntry>();
        private readonly List<IDetectable> _detectedObjectKeys = new List<IDetectable>();
        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            MonoBehaviourMaster.Instance.OnFixedUpdate += OnFixedUpdate;
        }

        public void RegisterOrUpdateDetectable(IDetectable detectable, float detectionRate, float detectionValue = 0f) {
            // if we don't have this detectable object
            if (!_detectedObjects.ContainsKey(detectable)) {
                _detectedObjects.Add(detectable, new DetectableEntry() {
                    Detectable = detectable,
                });
                OnDetectedObjectRegistered?.Invoke(detectable);
            }
            // set the detection value or increment
            if(detectionValue > 0f) {
                _detectedObjects[detectable].CurrentDetectionValue = detectionValue;
            } else {
                _detectedObjects[detectable].CurrentDetectionValue += Time.deltaTime * detectionRate;
            }
            // mark this object detected this frame
            _detectedObjects[detectable].DetectedThisFrame = true;
        }

        public void DeregisterDetectable(IDetectable detectable) {
            // ensure the detectable exists in registry
            if (!_detectedObjects.ContainsKey(detectable)) {
                return;
            }
            _detectedObjects.Remove(detectable);
            OnDetectedObjectDeregistered?.Invoke(detectable);
        }

        private void OnFixedUpdate() {
            ProcessAllDetectedObjects();
        }

        private void ProcessAllDetectedObjects() {
            // build separate list for iterating through keys
            _detectedObjectKeys.AddRange(_detectedObjects.Keys);
            foreach (IDetectable key in _detectedObjectKeys) {
                ProcessDetectedObject(_detectedObjects[key]);
            }
            // re-clear. I'm not a huge fan of this since this happens every frame for every NPC
            _detectedObjectKeys.Clear();
        }

        private void ProcessDetectedObject(DetectableEntry entry) {
            // if this object wasn't noticed this frame
            if (!entry.DetectedThisFrame) {
                entry.CurrentDetectionValue -= Time.deltaTime;
            }
            entry.DetectedThisFrame = false; // reset the detected this frame flag
            // if the detection value has reached 0, remove
            if (entry.CurrentDetectionValue <= 0f) {
                DeregisterDetectable(entry.Detectable);
            }
            // if this object has hit the detection threshold
            if (entry.CurrentDetectionValue >= DetectedThreshold) {
                Debug.Log("Detected object {}!");
                OnDetectedObject?.Invoke(entry.Detectable);
            }
        }
    }
}
