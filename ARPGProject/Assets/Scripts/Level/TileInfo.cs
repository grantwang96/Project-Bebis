using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class TileInfo {

        public int X { get; }
        public int Y { get; }
        public ITileOccupant TileOccupant { get; private set; } // something that might be blocking this tile

        private readonly List<IInteractable> _interactables = new List<IInteractable>();

        public TileInfo(int x, int y) {
            X = x;
            Y = y;
        }

        public void SetOccupant(ITileOccupant occupant) {
            TileOccupant = occupant;
        }

        public void AddInteractable(IInteractable interactable) {
            if (_interactables.Contains(interactable)) {
                return;
            }
            _interactables.Add(interactable);
        }

        public void RemoveInteractable(IInteractable interactable) {
            _interactables.Remove(interactable);
        }

        public IInteractable GetTopMostInteractable() {
            if(_interactables.Count == 0) {
                return null;
            }
            return _interactables[_interactables.Count - 1];
        }
    }
}
