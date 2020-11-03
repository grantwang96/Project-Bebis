using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class NPCActionInfoProvider : MonoBehaviour, IActionControllerInfoProvider {

        private List<CharacterActionEntry> _allActionEntries = new List<CharacterActionEntry>();
        private readonly Dictionary<string, ICharacterActionData> _actions = new Dictionary<string, ICharacterActionData>();

        public event Action<ICharacterActionData, CharacterActionContext> OnActionAttempted;

        public void Initialize() {
            _actions.Clear();
            for(int i = 0; i < _allActionEntries.Count; i++) {
                _actions.Add(_allActionEntries[i].Id, _allActionEntries[i].ActionData);
            }
        }

        public void AttemptAction(string actionId, CharacterActionContext context) {
            if(!_actions.TryGetValue(actionId, out ICharacterActionData characterActionData)) {
                Debug.LogError($"[{this.name}]: Could not retrieve action data for id {actionId}!");
                return;
            }
            OnActionAttempted?.Invoke(characterActionData, context);
        }

        [System.Serializable]
        protected class CharacterActionEntry {

            [SerializeField] private string _id;
            [SerializeField] private CharacterActionData _baseCharacterActionData;

            public string Id => _id;
            public ICharacterActionData ActionData => _baseCharacterActionData;
        }
    }
}
