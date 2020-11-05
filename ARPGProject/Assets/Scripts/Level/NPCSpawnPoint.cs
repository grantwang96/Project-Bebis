using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public class NPCSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string _npcPrefabId;
        [SerializeField] private string _npcUniqueId;

        private void Awake() {
            ManagerMaster.LevelDataManager.RegisterNPCSpawnPoint(name, this);
        }

        public void Spawn() {
            NPCCharacterInitData initData = new NPCCharacterInitData();
            initData.UniqueId = _npcUniqueId;
            initData.SpawnLocation = transform.position;
            ManagerMaster.CharactersManager.SpawnCharacter(_npcPrefabId, initData);
        }
    }
}
