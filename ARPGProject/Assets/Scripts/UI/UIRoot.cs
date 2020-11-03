using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public class UIRoot : MonoBehaviour
    {
        public static UIRoot Instance { get; private set; }

        [SerializeField] private List<UILayer> _layers = new List<UILayer>();
        private readonly Dictionary<UILayerId, UILayer> _uiLayers = new Dictionary<UILayerId, UILayer>();

        private void Awake() {
            Instance = this;
            _uiLayers.Clear();
            for (int i = 0; i < _layers.Count; i++) {
                _uiLayers.Add(_layers[i].UILayerId, _layers[i]);
            }
        }

        public UILayer GetUILayerFromId(UILayerId layerId) {
            return _uiLayers[layerId];
        }
    }
}
