using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Winston
{
    public interface IAssetManager
    {
        bool TryGetPrefab(string prefabId, out GameObject go);

        event Action<string, GameObject> OnPrefabRetrieved;
        event Action<string> OnFailedToRetrievePrefab;
    }

    public class AssetManager : MonoBehaviour, IAssetManager
    {
        public static IAssetManager Instance { get; private set; }

        public event Action<string, GameObject> OnPrefabRetrieved;
        public event Action<string> OnFailedToRetrievePrefab;

        [SerializeField] private List<PrefabEntry> _pooledObjectEntries = new List<PrefabEntry>();
        [SerializeField] private List<PrefabEntry> _uiDisplayPrefabEntries = new List<PrefabEntry>();
        private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        private void Awake() {
            Instance = this;
            _prefabs.Clear();
            LoadPrefabEntries(_pooledObjectEntries);
            LoadPrefabEntries(_uiDisplayPrefabEntries);
        }

        private void LoadPrefabEntries(List<PrefabEntry> prefabEntries) {
            for (int i = 0; i < prefabEntries.Count; i++) {
                _prefabs.Add(prefabEntries[i].PrefabId, prefabEntries[i].GO);
            }
        }

        public bool TryGetPrefab(string prefabId, out GameObject go) {
            return _prefabs.TryGetValue(prefabId, out go);
        }

        [System.Serializable]
        private class PrefabEntry
        {
            [SerializeField] private string _prefabId;
            [SerializeField] private GameObject _go;

            public string PrefabId => _prefabId;
            public GameObject GO => _go;
        }
    }
}
