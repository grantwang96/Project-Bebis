using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TempCharacterAnimatorOverrider : MonoBehaviour {

        [SerializeField] private PlayerCharacter _playerCharacter;
        public List<AnimationClipOverride> HackData1;
        public List<AnimationClipOverride> HackData2;

        private bool _set1;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.P)) {
                ToggleAnimationSet();
            }
        }

        private void ToggleAnimationSet() {
            _set1 = !_set1;
            List<AnimationClipOverride> overrides = _set1 ? HackData1 : HackData2;
            OverrideAnimationController(overrides);
        }

        private void OverrideAnimationController(List<AnimationClipOverride> overrides) {
            _playerCharacter.AnimationController.OverrideAnimationController(overrides);
        }
    }
}
