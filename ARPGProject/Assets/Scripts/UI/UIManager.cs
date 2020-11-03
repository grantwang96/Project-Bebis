using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public interface IUIManager
    {
        IUIDisplay CreateUIDisplay(string uiPrefabId, UILayerId layerId);
        void RemoveUIDisplay(string uiPrefabId);
    }

    public class UIManager : IUIManager
    {
        private readonly Dictionary<string, IUIDisplay> _activeUIDisplays = new Dictionary<string, IUIDisplay>();

        public IUIDisplay CreateUIDisplay(string uiPrefabId, UILayerId layerId) {
            if (_activeUIDisplays.ContainsKey(uiPrefabId)) {
                Debug.LogWarning($"[{nameof(UIManager)}]: Already contains UI Display prefab with Id {uiPrefabId}!");
                return _activeUIDisplays[uiPrefabId];
            }
            if(!AssetManager.Instance.TryGetPrefab(uiPrefabId, out GameObject go)) {
                Debug.LogError($"[{nameof(UIManager)}]: Could not retrieve UI Display prefab with Id {uiPrefabId}!");
                return null;
            }
            IUIDisplay uiDisplay = go.GetComponent<IUIDisplay>();
            if (uiDisplay == null) {
                Debug.LogError($"[{nameof(UIManager)}]: Retrieved object with Id {uiPrefabId} was not of type {nameof(IUIDisplay)}!");
                return null;
            }
            UILayer uiLayer = UIRoot.Instance.GetUILayerFromId(layerId);
            GameObject clonedDisplayGO = UnityEngine.Object.Instantiate(go, uiLayer.transform);
            IUIDisplay clonedDisplay = clonedDisplayGO.GetComponent<IUIDisplay>();
            _activeUIDisplays.Add(uiPrefabId, clonedDisplay);
            return clonedDisplay;
        }

        public bool TryGetUIDisplay(string uiPrefabId, out IUIDisplay uiDisplay) {
            return _activeUIDisplays.TryGetValue(uiPrefabId, out uiDisplay);
        }

        public void RemoveUIDisplay(string uiPrefabId) {
            if(!_activeUIDisplays.TryGetValue(uiPrefabId, out IUIDisplay uiDisplay)) {
                return;
            }
            uiDisplay.OnHide();
            _activeUIDisplays.Remove(uiPrefabId);
        }
    }

    [System.Serializable]
    public class UIDisplayIdEntry
    {
        [SerializeField] private string _uiDisplayId;
        [SerializeField] private UILayerId _uiLayerId;

        public string UIDisplayId => _uiDisplayId;
        public UILayerId UILayerId => _uiLayerId;
    }
}
