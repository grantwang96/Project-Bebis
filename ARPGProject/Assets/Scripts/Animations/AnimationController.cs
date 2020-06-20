using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public interface IAnimationController {

        event Action<ActionStatus> OnActionStatusUpdated;
        event Action<AnimationState> OnAnimationStateUpdated;

        void UpdateAnimationState(AnimationData data);
        void OverrideAnimationController(IReadOnlyList<AnimationClipOverride> overrideClips);
        AnimatorClipInfo[] GetCurrentAnimatorClipInfos();
        Vector3 DeltaPosition();
    }

    [System.Serializable]
    public class AnimationData {
        [SerializeField] private string _animationName;
        [SerializeField] private string[] _triggers;
        [SerializeField] private string[] _resetTriggers;
        [SerializeField] private SerializedStringInt[] _intValues;
        [SerializeField] private SerializedStringFloat[] _floatValues;
        [SerializeField] private SerializedStringBool[] _boolValues;

        public string AnimationName => _animationName;
        public string[] Triggers => _triggers;
        public string[] ResetTriggers => _resetTriggers;
        public SerializedStringInt[] IntValues => _intValues;
        public SerializedStringFloat[] FloatValues => _floatValues;
        public SerializedStringBool[] BoolValues => _boolValues;
    }

    public enum AnimationState {
        Started,
        InProgress,
        Completed
    }
}
