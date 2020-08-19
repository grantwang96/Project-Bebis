using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class CharacterHurtboxController : HurtboxController {

        [SerializeField] private GameObject _characterGO;

        private void Awake() {
            SetCharacter();
        }

        protected override void OnEnable() {
            SetCharacter();
            base.OnEnable();
        }

        private void SetCharacter() {
            if (_character == null) {
                _character = _characterGO.GetComponent<ICharacter>();
            }
        }
    }
}
