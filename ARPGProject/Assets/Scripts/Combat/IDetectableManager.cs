using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface IDetectableManager : ICharacterComponent
    {
        void RegisterOrUpdateDetectable(IDetectable detectable, float detectionValue = 0f);
        void DeregisterDetectable(IDetectable detectable);
    }

    public class DetectableEntry
    {
        public IDetectable Detectable;
        public float CurrentDetectionValue;
        public bool DetectedThisFrame;
    }
}
