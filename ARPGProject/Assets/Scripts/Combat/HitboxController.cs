using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class HitboxController : CharacterComponent {

        [SerializeField] private Hitbox[] _hitBoxObjects;
        private Dictionary<string, Hitbox> _hitBoxes = new Dictionary<string, Hitbox>();
        public IReadOnlyDictionary<string, Hitbox> HitBoxes => _hitBoxes;

        protected override void Awake() {
            base.Awake();
            InitializeHitboxList();
        }

        private void InitializeHitboxList() {
            _hitBoxes.Clear();
            for (int i = 0; i < _hitBoxObjects.Length; i++) {
                _hitBoxes.Add(_hitBoxObjects[i].name, _hitBoxObjects[i]);
            }
        }

        public virtual void SetHitboxInfo(string id, HitboxInfo info) {
            if (!_hitBoxes.TryGetValue(id, out Hitbox hitBox)) {
                CustomLogger.Error(name, $"Could not retrieve hitbox with id {id}!");
                return;
            }
            hitBox.Initialize(info);
        }
    }
}
