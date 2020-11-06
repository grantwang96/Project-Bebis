using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Winston;

namespace Bebis
{
    public class TriggerNPCSpawner : MonoBehaviour
    {
        [SerializeField] private string _prefabId;
        [SerializeField] private string _overrideUniqueId;
        [SerializeField] private string _spawnPointId;
        [SerializeField] private int _count;

        private int _totalSpawned = 0;

        private void OnTriggerEnter(Collider other) {
            if(other.transform != PlayerCharacter.Instance.MoveController.Body) {
                return;
            }
            if(!ManagerMaster.LevelDataManager.TryGetNPCSpawnPoint(_spawnPointId, out NPCSpawnPoint spawnPoint)) {
                Debug.LogError($"[{name}]: Invalid spawnpoint id \"{_spawnPointId}\"");
                return;
            }
            spawnPoint.Spawn(_prefabId);
            _totalSpawned = 0;
            if(_totalSpawned >= _count) {
                gameObject.SetActive(false);
            }
        }
    }
}
