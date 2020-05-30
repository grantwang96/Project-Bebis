using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public class CharacterComponent : MonoBehaviour {

        [SerializeField] private GameObject _characterGO;
        protected ICharacter _character;

        protected virtual void Awake() {
            _character = _characterGO.GetComponent<ICharacter>();
            if(_character == null) {
                CustomLogger.Error(name, $"Did not receive [{nameof(ICharacter)}] component!");
            }
        }
    }
}
