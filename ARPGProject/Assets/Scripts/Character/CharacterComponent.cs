using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {

    public class PlayerCharacterComponent {

        protected PlayerCharacter _character;

        public PlayerCharacterComponent(PlayerCharacter character) {
            _character = character as PlayerCharacter;
            if (_character == null) {
                CustomLogger.Error(nameof(PlayerCharacterComponent), $"Did not receive character of type [{nameof(PlayerCharacter)}]");
            }
        }
    }
}
