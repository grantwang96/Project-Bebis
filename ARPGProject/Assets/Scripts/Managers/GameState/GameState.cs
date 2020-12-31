using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Winston {
    public class GameState : AbstractState {

        [SerializeField] private string _sceneName;
        [SerializeField] private List<PooledObjectCountEntry> _pooledObjectsToSpawn = new List<PooledObjectCountEntry>();
        [SerializeField] private List<UIDisplayIdEntry> _uiDisplayIds = new List<UIDisplayIdEntry>();

        private readonly List<IPooledObject> _spawnedPooledObjects = new List<IPooledObject>();
        private readonly List<IUIDisplay> _uiDisplays = new List<IUIDisplay>();

        // set up the state
        protected override void Enter() {
            // Debug.Log($"[{name}]: Entered game state {StateId}!");
            GeneratePooledObjects();
            CreateUIDisplays();
            base.Enter();
        }

        protected override bool IsReadyToEnter() {
            bool readyToEnter = base.IsReadyToEnter();
            // if we require a scene change, load the scene first and wait until it is finished
            if (readyToEnter && !string.IsNullOrEmpty(_sceneName)) {
                // Debug.Log($"[{name}]: Load scene: {_sceneName}");
                ManagerMaster.SceneController.OnSceneLoaded += OnSceneLoaded;
                ManagerMaster.SceneController.LoadScene(_sceneName);
                return false;
            }
            return true;
        }

        // if we're waiting on a scene change
        private void OnSceneLoaded(bool success) {
            ManagerMaster.SceneController.OnSceneLoaded -= OnSceneLoaded;
            if (!success) {
                Debug.LogError($"[{name}]: Scene {_sceneName} failed to load!");
                FailToEnter();
                return;
            }
            Enter();
        }

        protected override void Exit() {
            // Debug.Log($"[{name}]: Exited game state {StateId}!");
            base.Exit();
            RemoveUIDisplays();
            RemovePooledObjects();
        }

        private void GeneratePooledObjects() {
            for(int i = 0; i < _pooledObjectsToSpawn.Count; i++) {
                PooledObjectCountEntry entry = _pooledObjectsToSpawn[i];
                ManagerMaster.PooledObjectManager.RegisterPooledObject(entry.Id, entry.Count);
            }
        }

        private void CreateUIDisplays() {
            for(int i = 0; i < _uiDisplayIds.Count; i++) {
                IUIDisplay uiDisplay = ManagerMaster.UIManager.CreateUIDisplay(_uiDisplayIds[i].UIDisplayId, _uiDisplayIds[i].UILayerId);
                if(uiDisplay == null) {
                    Debug.LogError($"[{name}]: Failed to load UI Display object {_uiDisplayIds[i].UIDisplayId}!");
                    continue;
                }
                _uiDisplays.Add(uiDisplay);
                uiDisplay.Initialize(UIDisplayInitializationData.Empty);
            }
        }

        private void RemovePooledObjects() {
            for(int i = 0; i < _pooledObjectsToSpawn.Count; i++) {
                PooledObjectCountEntry entry = _pooledObjectsToSpawn[i];
                ManagerMaster.PooledObjectManager.UnregisterPooledObject(entry.Id);
            }
        }

        private void RemoveUIDisplays() {
            _uiDisplays.Clear();
            for (int i = 0; i < _uiDisplayIds.Count; i++) {
                ManagerMaster.UIManager.RemoveUIDisplay(_uiDisplayIds[i].UIDisplayId);
            }
        }

        [System.Serializable]
        private class PooledObjectCountEntry
        {
            public string Id;
            public int Count;
        }
    }

    [System.Serializable]
    public class AbstractStateTransition {

        [SerializeField] private string _transitionId;
        [SerializeField] private AbstractState _state;

        public string TransitionId => _transitionId;
        public AbstractState State => _state;
    }

}