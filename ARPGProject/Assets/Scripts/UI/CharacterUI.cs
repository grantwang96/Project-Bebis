using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class CharacterUI : CharacterComponent {

        [SerializeField] private NPCTargetManager _npcTargetManager;
        [SerializeField] private FillBar _healthBar;
        [SerializeField] private FillBar _awarenessLevel;

        private HostileEntry _playerHostileEntry;

        private void Start() {
            _character.Damageable.OnCurrentHealthChanged += OnCurrentHealthChanged;
            _npcTargetManager.OnNewHostileRegistered += OnNewHostileRegistered;
        }

        private void OnDestroy() {
            _character.Damageable.OnCurrentHealthChanged -= OnCurrentHealthChanged;
            _npcTargetManager.OnNewHostileRegistered -= OnNewHostileRegistered;
        }

        private void OnCurrentHealthChanged(int newHealth) {
            _healthBar.SetFillPercent((float)newHealth / _character.Damageable.MaxHealth);
        }

        private void OnNewHostileRegistered(HostileEntry entry) {
            if(entry.Hostile != PlayerCharacter.Instance) {
                return;
            }
            _playerHostileEntry = entry;
        }

        private void OnRegisteredHostilesUpdated() {
            if(_npcTargetManager.CurrentTarget != null) {
                return;
            }
            if (_playerHostileEntry != null && _npcTargetManager.RegisteredHostiles.Contains(_playerHostileEntry)) {

            }
        }
    }
}
