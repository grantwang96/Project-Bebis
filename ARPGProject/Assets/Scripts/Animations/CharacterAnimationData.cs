using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public partial class GameplayValues {
        public partial class Animation {
            public const string TorsoIdleS = "torso_idle_s";
            public const string TorsoIdleE = "torso_idle_e";
            public const string TorsoIdleN = "torso_idle_n";
            public const string TorsoIdleW = "torso_idle_w";
            public const string TorsoIdleNE = "torso_idle_ne";
            public const string TorsoIdleSE = "torso_idle_se";
            public const string TorsoIdleSW = "torso_idle_sw";
            public const string TorsoIdleNW = "torso_idle_nw";

            public enum AnimationIds {
                torso_idle_s,
                torso_idle_e,
                torso_idle_n,
                torso_idle_w,
                torso_idle_ne,
                torso_idle_se,
                torso_idle_sw,
                torso_idle_nw
            }
        }
    }

    [System.Serializable]
    public class AnimationClipOverride {
        [SerializeField] private GameplayValues.Animation.AnimationIds _id;
        [SerializeField] private AnimationClip _animationClip;

        public string Id => _id.ToString();
        public AnimationClip AnimationClip => _animationClip;
    }
}
