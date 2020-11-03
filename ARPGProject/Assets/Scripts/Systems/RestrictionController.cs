using System.Collections;
using System.Collections.Generic;
using System;

namespace Bebis
{
    public class RestrictionController
    {
        public bool Restricted => _totalRestrictions > 0;

        public event Action OnRestrictionUpdated;

        private readonly Dictionary<string, int> _restrictionsMap = new Dictionary<string, int>();
        private int _totalRestrictions;

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
            _totalRestrictions = _totalRestrictions > 0 ? _totalRestrictions - 1 : 0;
            OnRestrictionUpdated?.Invoke();
        }

        public bool ContainsId(string id) {
            return _restrictionsMap.ContainsKey(id);
        }
    }
}
