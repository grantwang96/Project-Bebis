using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public class PlayerSpawner : MonoBehaviour
    {
        private const string PlayerPrefabId = "PlayerCharacter";
        private const string PlayerUniqueId = "PlayerId";

        private void Awake() {
            ManagerMaster.LevelDataManager.RegisterPlayerSpawner(name, this);
        }

        public void Spawn() {
            PlayerCharacterInitializationData initData = new PlayerCharacterInitializationData();
            initData.UniqueId = PlayerUniqueId;
            initData.SpawnLocation = transform.position;
            ManagerMaster.CharactersManager.SpawnCharacter(PlayerPrefabId, initData);
        }
    }
}
