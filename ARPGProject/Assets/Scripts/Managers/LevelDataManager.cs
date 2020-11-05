using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public interface ILevelDataManager
    {
        void RegisterPlayerSpawner(string spawnerId, PlayerSpawner spawner);
        bool TryGetPlayerSpawner(string spawnerId, out PlayerSpawner spawner);
        void RegisterNPCSpawnPoint(string id, NPCSpawnPoint spawnpoint);
        bool TryGetNPCSpawnPoint(string id, out NPCSpawnPoint spawnpoint);
        void Initialize();
        void Clear();
    }

    public class LevelDataManager : ILevelDataManager
    {
        private readonly Dictionary<string, PlayerSpawner> _playerSpawners = new Dictionary<string, PlayerSpawner>();
        private readonly Dictionary<string, NPCSpawnPoint> _npcSpawnPoints = new Dictionary<string, NPCSpawnPoint>();

        public void Initialize() {
            Clear();
        }

        public void RegisterPlayerSpawner(string spawnerId, PlayerSpawner spawner) {
            if (_playerSpawners.ContainsKey(spawnerId)) {
                return;
            }
            _playerSpawners.Add(spawnerId, spawner);
        }

        public bool TryGetPlayerSpawner(string spawnerId, out PlayerSpawner spawner) {
            return _playerSpawners.TryGetValue(spawnerId, out spawner);
        }

        public void RegisterNPCSpawnPoint(string id, NPCSpawnPoint spawnpoint) {
            if (_npcSpawnPoints.ContainsKey(id)) {
                return;
            }
            _npcSpawnPoints.Add(id, spawnpoint);
        }

        public bool TryGetNPCSpawnPoint(string id, out NPCSpawnPoint spawnpoint) {
            return _npcSpawnPoints.TryGetValue(id, out spawnpoint);
        }

        public void Clear() {
            _playerSpawners.Clear();
            _npcSpawnPoints.Clear();
        }
    }
}
