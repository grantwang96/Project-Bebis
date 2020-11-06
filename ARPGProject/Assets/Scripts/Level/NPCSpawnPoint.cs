using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public class NPCSpawnPoint : MonoBehaviour
    {
        private void Awake() {
            ManagerMaster.LevelDataManager.RegisterNPCSpawnPoint(name, this);
        }

        public void Spawn(string npcPrefabId, string overrideUniqueId = "") {
            NPCCharacterInitData initData = new NPCCharacterInitData();
            if (!string.IsNullOrEmpty(overrideUniqueId)) {
                initData.UniqueId = overrideUniqueId;
            } else {
                initData.UniqueId = NPCCharacter.GenerateUniqueId(npcPrefabId);
            }
            initData.SpawnLocation = transform.position;
            initData.SpawnRotation = transform.rotation;
            ManagerMaster.CharactersManager.SpawnCharacter(npcPrefabId, initData);
        }
    }
}
