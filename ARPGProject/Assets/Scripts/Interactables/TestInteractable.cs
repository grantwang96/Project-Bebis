using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TestInteractable2D : MonoBehaviour, IInteractable {

        [SerializeField] private IntVector3 _position;

        private void Start() {
            IntVector3 position = LevelDataManagerV2.GetMapPosition(transform.position);
            _position = position;
            LevelDataManagerV2.Instance.AddTileInteractable(position.x, position.y, this);
        }

        public void OnInteractStart(ICharacter character) {
            CustomLogger.Log(nameof(TestInteractable2D), $"Character {character.MoveController.Body.name} has started interacting with {name}");
        }

        public void OnInteractHold(ICharacter character) {
            CustomLogger.Log(nameof(TestInteractable2D), $"Character {character.MoveController.Body.name} continues interacting with {name}");
        }

        public void OnInteractRelease(ICharacter character) {
            CustomLogger.Log(nameof(TestInteractable2D), $"Character {character.MoveController.Body.name} has stopped interacting with {name}");
        }
    }
}
