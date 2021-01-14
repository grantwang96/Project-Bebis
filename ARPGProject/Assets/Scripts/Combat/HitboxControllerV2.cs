using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class HitboxControllerV2 : MonoBehaviour, ICharacterComponent
    {
        public IReadOnlyDictionary<string, HitboxV2> HitBoxes => _hitBoxes;

        [SerializeField] private HitboxV2[] _hitBoxObjects;
        private Dictionary<string, HitboxV2> _hitBoxes = new Dictionary<string, HitboxV2>();
        private ICharacterV2 _character;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            InitializeHitboxList();
        }

        private void InitializeHitboxList() {
            _hitBoxes.Clear();
            for (int i = 0; i < _hitBoxObjects.Length; i++) {
                _hitBoxes.Add(_hitBoxObjects[i].name, _hitBoxObjects[i]);
            }
        }

        public virtual void SetHitboxInfo(string id, HitboxInfoV2 info) {
            if (!_hitBoxes.TryGetValue(id, out HitboxV2 hitBox)) {
                CustomLogger.Error(name, $"Could not retrieve hitbox with id {id}!");
                return;
            }
            hitBox.Initialize(info);
        }

        public void Clear(string id) {
            if (!_hitBoxes.TryGetValue(id, out HitboxV2 hitBox)) {
                CustomLogger.Error(name, $"Could not retrieve hitbox with id {id}!");
                return;
            }
            hitBox.Clear();
        }
    }
}
