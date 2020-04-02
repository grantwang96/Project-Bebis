using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class HitboxController : MonoBehaviour {

        [SerializeField] private Transform _hitBoxAnchor;
        [SerializeField] private Hitbox[] _hitBoxObjects;
        private Dictionary<string, Hitbox> _hitBoxes = new Dictionary<string, Hitbox>();
        private PlayerCharacter _character;

        private void Awake() {
            _character = GetComponent<PlayerCharacter>();
            InitializeHitboxList();
        }

        private void Update() {
            SetAnchorRotation();
        }

        private void SetAnchorRotation() {
            if (!_character.ActionController.Permissions.HasFlag(ActionPermissions.Rotation)) {
                return;
            }
            Vector3 moveVector = _character.MoveController.Rotation;
            float angle = Mathf.Atan2(-moveVector.x, moveVector.y);
            angle *= Mathf.Rad2Deg;
            _hitBoxAnchor.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void InitializeHitboxList() {
            _hitBoxes.Clear();
            for (int i = 0; i < _hitBoxObjects.Length; i++) {
                _hitBoxes.Add(_hitBoxObjects[i].name, _hitBoxObjects[i]);
            }
        }

        public void SetHitboxInfo(string id, HitboxInfo info) {
            if(_hitBoxes.TryGetValue(id, out Hitbox hitBox)) {
                hitBox.SetInfo(info);
            }
        }
    }
}
