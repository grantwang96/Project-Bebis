using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public interface IDetectableManager : ICharacterComponent
    {
        void RegisterOrUpdateDetectable(IDetectable detectable, float detectionRate, float detectionValue = 0f);
        void DeregisterDetectable(IDetectable detectable);

        // event to inform that an object has been registered
        event Action<IDetectable> OnDetectedObjectRegistered;
        // event to inform tha an object has been deregistered
        event Action<IDetectable> OnDetectedObjectDeregistered;
        // event to inform that an object has been detected
        event Action<IDetectable> OnDetectedObject;
    }

    public class DetectableEntry
    {
        public IDetectable Detectable;
        public float CurrentDetectionValue;
        public bool DetectedThisFrame;
    }
}
