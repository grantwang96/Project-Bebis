using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Winston
{
    public interface IPooledObjectManager
    {
        void RegisterPooledObject(string prefabId,  int initialCount, string poolId = "", Action<bool> onRegisterComplete = null);
        void UnregisterPooledObject(string prefabId);
        IPooledObject UsePooledObject(string poolId);
        void ReturnPooledObject(string poolId, IPooledObject pooledObject);
    }

    public class PooledObjectManager : IPooledObjectManager
    {
        private const int MaxEntryCount = 200;

        private readonly Dictionary<string, PooledObjectEntry> _pooledObjects = new Dictionary<string, PooledObjectEntry>();

        public void RegisterPooledObject(string prefabId, int initialCount, string newPoolId = "", Action<bool> onRegisterComplete = null) {
            // attempt to retrieve the base gameobject
            if(!AssetManager.Instance.TryGetPrefab(prefabId, out GameObject go)) {
                Debug.LogError($"[{nameof(PooledObjectManager)}]: Could not retrieve gameobject with prefab id \"{prefabId}\"!");
                onRegisterComplete?.Invoke(false);
                return;
            }
            // attempt to convert to Pooled Object
            IPooledObject pooledObject = go.GetComponent<IPooledObject>();
            if(pooledObject == null) {
                Debug.LogError($"[{nameof(PooledObjectManager)}]: Object retrieved with id \"{prefabId}\" was not of type {nameof(IPooledObject)}!");
                onRegisterComplete?.Invoke(false);
                return;
            }
            // get pool id for the object
            string poolId = string.IsNullOrEmpty(newPoolId) ? prefabId : newPoolId;
            if (_pooledObjects.ContainsKey(poolId)) {
                Debug.LogWarning($"[{nameof(PooledObjectManager)}]: Already registered pooled object with id \"{prefabId}\"!");
                onRegisterComplete?.Invoke(true);
                return;
            }
            // add a new entry to the registry
            PooledObjectEntry newEntry = new PooledObjectEntry() {
                Id = poolId,
                GO = go,
                Base = pooledObject,
            };
            _pooledObjects.Add(newEntry.Id, newEntry);
            ClonePooledObject(poolId, initialCount);
            onRegisterComplete?.Invoke(true);
        }

        public void UnregisterPooledObject(string poolId) {
            if (!_pooledObjects.TryGetValue(poolId, out PooledObjectEntry entry)) {
                return;
            }
            entry.Reduce(entry.TotalCount);
            _pooledObjects.Remove(poolId);
        }

        public IPooledObject UsePooledObject(string poolId) {
            if(!_pooledObjects.TryGetValue(poolId, out PooledObjectEntry entry)) {
                return null;
            }
            return entry.UsePooledObject();
        }

        public void ClonePooledObject(string poolId, int count) {
            if(!_pooledObjects.TryGetValue(poolId, out PooledObjectEntry entry)) {
                Debug.LogError($"[{nameof(PooledObjectManager)}]: Could not retrieve pooled object with pool id \"{poolId}\"!");
                return;
            }
            entry.Clone(count);
        }

        public void ReturnPooledObject(string poolId, IPooledObject pooledObject) {
            if (!_pooledObjects.TryGetValue(poolId, out PooledObjectEntry entry)) {
                Debug.LogError($"[{nameof(PooledObjectManager)}]: Could not retrieve pooled object with pool id \"{poolId}\"!");
                return;
            }
            entry.ReturnPooledObject(pooledObject);
        }

        private class PooledObjectEntry
        {
            public string Id;
            public GameObject GO;
            public IPooledObject Base;

            private readonly List<IPooledObject> _availablePooledObjects = new List<IPooledObject>();
            private readonly List<IPooledObject> _inUsePooledObjects = new List<IPooledObject>();

            public int TotalCount => _availablePooledObjects.Count + _inUsePooledObjects.Count;

            public IPooledObject UsePooledObject() {
                if(_availablePooledObjects.Count == 0) {
                    Debug.LogWarning($"[{nameof(PooledObjectManager)}/{Id}]: No more available pooled objects to use!");
                    return null;
                }
                IPooledObject pooledObject = _availablePooledObjects[0];
                _availablePooledObjects.RemoveAt(0);
                _inUsePooledObjects.Add(pooledObject);
                return pooledObject;
            }

            public void ReturnPooledObject(IPooledObject pooledObject) {
                if (!_inUsePooledObjects.Contains(pooledObject)) {
                    return;
                }
                _inUsePooledObjects.Remove(pooledObject);
                _availablePooledObjects.Add(pooledObject);
            }

            public void Clone(int count) {
                // clone required amount
                for (int i = 0; i < count; i++) {
                    IPooledObject clonedPO = Base.Spawn();
                    clonedPO.SetActive(false);
                    _availablePooledObjects.Add(clonedPO);
                }
            }

            public void Reduce(int count) {
                for(int i = 0; i < count; i++) {
                    IPooledObject toBeRemoved;
                    // prioritize objects not in use first
                    if (_availablePooledObjects.Count > 0) {
                        toBeRemoved = _availablePooledObjects[0];
                        _availablePooledObjects.RemoveAt(0);
                    } else {
                        toBeRemoved = _inUsePooledObjects[0];
                        _availablePooledObjects.RemoveAt(0);
                    }
                    toBeRemoved.Despawn();
                }
            }
        }
    }
}
