using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Bebis
{
    [System.Serializable]
    public class RestrictionController
    {
        public bool Restricted => _totalRestrictions > 0;

        public event Action OnRestrictionUpdated;

        private readonly Dictionary<string, int> _restrictionsMap = new Dictionary<string, int>();
        [SerializeField] private int _totalRestrictions;

        public void AddRestriction(string id) {
            if (!_restrictionsMap.ContainsKey(id)) {
                _restrictionsMap.Add(id, 0);
            }
            _restrictionsMap[id]++;
            _totalRestrictions++;
            OnRestrictionUpdated?.Invoke();
        }

        public void RemoveRestriction(string id) {
            if (!_restrictionsMap.ContainsKey(id)) {
                return;
            }
            _restrictionsMap[id]--;
            if(_restrictionsMap[id] <= 0) {
                _restrictionsMap.Remove(id);
            }
            _totalRestrictions--;
            if (_totalRestrictions < 0) {
                _totalRestrictions = 0;
            }
            OnRestrictionUpdated?.Invoke();
        }

        public void RemoveRestrictionAll(string id) {
            if (!_restrictionsMap.ContainsKey(id)) {
                return;
            }
            int count = _restrictionsMap[id];
            _restrictionsMap.Remove(id);
            _totalRestrictions -= count;
            if(_totalRestrictions < 0) {
                _totalRestrictions = 0;
            }
            OnRestrictionUpdated?.Invoke();
        }

        public bool ContainsId(string id) {
            return _restrictionsMap.ContainsKey(id);
        }

        public void Clear() {
            _restrictionsMap.Clear();
            _totalRestrictions = 0;
        }
    }
}
