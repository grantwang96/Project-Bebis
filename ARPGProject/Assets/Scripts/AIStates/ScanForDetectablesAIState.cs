using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class ScanForDetectablesAIState : AIStateV2
    {
        [SerializeField] private NPCUnitController _npcUnitController;
        [SerializeField] private NPCVisionV2 _npcVision;
        [SerializeField] private DetectableTags _tagsToScanFor;
        [SerializeField] private AIStateV2 _onDetectedObjectAIState;

        protected override void OnEnter() {
            base.OnEnter();
            MonoBehaviourMaster.Instance.OnUpdate += OnUpdate;
            _npcUnitController.DetectableManager.OnDetectedObject += OnDetectedObject;
        }

        protected override void OnExit() {
            base.OnExit();
            MonoBehaviourMaster.Instance.OnUpdate -= OnUpdate;
        }

        private void OnUpdate() {
            IReadOnlyList<IDetectable> detectedObjects = _npcVision.ScanForDetectables(_tagsToScanFor);
            for(int i = 0; i < detectedObjects.Count; i++) {
                _npcUnitController.DetectableManager.RegisterOrUpdateDetectable(detectedObjects[i], 1f); // temp: replace with calculated detection rate (probably in NPCVision component)
            }
        }

        private void OnDetectedObject(IDetectable detectable) {
            
        }
    }
}
