using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface IAnimationController {

        void UpdateAnimationState(AnimationData data);
        void OverrideAnimationController(IReadOnlyList<AnimationClipOverride> overrideClips);
    }

    [System.Serializable]
    public class AnimationData {
        [SerializeField] private string _animationName;
        [SerializeField] private string[] _triggers;
        [SerializeField] private SerializedStringInt[] _intValues;
        [SerializeField] private SerializedStringFloat[] _floatValues;
        [SerializeField] private SerializedStringBool[] _boolValues;

        public string AnimationName => _animationName;
        public string[] Triggers => _triggers;
        public SerializedStringInt[] IntValues => _intValues;
        public SerializedStringFloat[] FloatValues => _floatValues;
        public SerializedStringBool[] BoolValues => _boolValues;
    }
}
