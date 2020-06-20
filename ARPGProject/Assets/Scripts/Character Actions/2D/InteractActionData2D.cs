using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    [CreateAssetMenu(menuName = "Character Actions/Interact2D")]
    public class InteractActionData2D : CharacterActionData {

        public override CharacterActionResponse Initiate(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            bool canInteract = CanInteract();
            canInteract &= TryGetInteractable(character.MoveController.Body.position, character.MoveController.Rotation, out IInteractable interactable);
            if (!canInteract) {
                return base.Initiate(character, state, context);
            }
            interactable.OnInteractStart(character);
            CharacterActionResponse response = new CharacterActionResponse(true, state);
            return response;
        }

        public override CharacterActionResponse Hold(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            bool canInteract = CanInteract();
            canInteract &= TryGetInteractable(character.MoveController.Body.position, character.MoveController.Rotation, out IInteractable interactable);
            if (!canInteract) {
                return base.Hold(character, state, context);
            }
            interactable.OnInteractHold(character);
            CharacterActionResponse response = new CharacterActionResponse(true, state);
            return response;
        }

        public override CharacterActionResponse Release(ICharacter character, ICharacterActionState state, CharacterActionContext context) {
            bool canInteract = CanInteract();
            canInteract &= TryGetInteractable(character.MoveController.Body.position, character.MoveController.Rotation, out IInteractable interactable);
            if (!canInteract) {
                return base.Release(character, state, context);
            }
            interactable.OnInteractRelease(character);
            CharacterActionResponse response = new CharacterActionResponse(true, state);
            return response;
        }

        // TODO: check if current action allows for interaction
        private static bool CanInteract() {
            return true;
        }

        private static bool TryGetInteractable(Vector2 position, Vector2 rotation, out IInteractable interactable) {
            interactable = null;
            TileInfo tileInfo = GetTileInfo(position, rotation);
            if (tileInfo == null) {
                return false;
            }
            interactable = tileInfo.GetTopMostInteractable();
            if (interactable == null) {
                return false;
            }
            return true;
        }

        private static TileInfo GetTileInfo(Vector2 position, Vector2 rotation) {
            IntVector3 mapPosition = LevelDataManager.GetMapPosition(position + rotation);
            TileInfo tileInfo = LevelDataManager.Instance.TileInfos[mapPosition.x][mapPosition.y];
            return tileInfo;
        }
    }

    public class InteractActionState2D : CharacterActionState {

        public InteractActionState2D(CharacterActionData data, ICharacter character) : base(data, character) {

        }
    }
}
