using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class LevelDataManager : MonoBehaviour {

        public static LevelDataManager Instance { get; private set; }

        [SerializeField] private int _mapWidth;
        [SerializeField] private int _mapHeight;

        public TileInfo[][] TileInfos { get; private set; }

        private void Awake() {
            Instance = this;
            InitializeMap();
        }

        private void InitializeMap() {
            TileInfos = new TileInfo[_mapWidth][];
            for(int i = 0; i < _mapWidth; i++) {
                TileInfos[i] = new TileInfo[_mapHeight];
                for(int j = 0; j < _mapHeight; j++) {
                    TileInfos[i][j] = new TileInfo(i, j);
                }
            }
        }

        public void SetTileOccupant(int x, int y, ITileOccupant occupant) {
            if(!IsWithinMap(x, y)) {
                CustomLogger.Warn(nameof(LevelDataManager), $"Coordinate {x}, {y} is outside map bounds!");
                return;
            }
            TileInfos[x][y].SetOccupant(occupant);
        }

        public void AddTileInteractable(int x, int y, IInteractable interactable) {
            if(!IsWithinMap(x, y)) {
                CustomLogger.Warn(nameof(LevelDataManager), $"Coordinate {x}, {y} is outside map bounds!");
                return;
            }
            TileInfos[x][y].AddInteractable(interactable);
        }

        public void RemoveTileInteractable(int x, int y, IInteractable interactable) {
            if (!IsWithinMap(x, y)) {
                CustomLogger.Warn(nameof(LevelDataManager), $"Coordinate {x}, {y} is outside map bounds!");
                return;
            }
            TileInfos[x][y].RemoveInteractable(interactable);
        }

        public bool IsWithinMap(int x, int y) {
            return x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight;
        }

        public static IntVector3 GetMapPosition(Vector2 position) {
            return new IntVector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        }
    }
}
