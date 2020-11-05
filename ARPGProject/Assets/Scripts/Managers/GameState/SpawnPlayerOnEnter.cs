using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public class SpawnPlayerOnEnter : GameStateListener
    {
        private const string DefaultPlayerSpawnerId = "DefaultPlayerSpawner";

        protected override void OnGameStateEntered(bool success) {
            base.OnGameStateEntered(success);
            // TEMP: replace this with data driven system
            if (!ManagerMaster.LevelDataManager.TryGetPlayerSpawner(DefaultPlayerSpawnerId, out PlayerSpawner spawner)) {
                Debug.LogError($"[{_gameState?.StateId ?? "none"}]: Could not retrieve spawner with id \"{DefaultPlayerSpawnerId}\"!");
                return;
            }
            spawner.Spawn();
        }
    }
}
