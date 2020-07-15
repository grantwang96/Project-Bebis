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
            _npcTargetManager.OnRegisteredHostilesUpdated += OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet += OnCurrentTargetUpdated;
            DisableAwarenessMeter();
        }

        private void OnDestroy() {
            _character.Damageable.OnCurrentHealthChanged -= OnCurrentHealthChanged;
            _npcTargetManager.OnNewHostileRegistered -= OnNewHostileRegistered;
            _npcTargetManager.OnRegisteredHostilesUpdated -= OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet -= OnCurrentTargetUpdated;
        }

        private void OnCurrentHealthChanged(int newHealth) {
            _healthBar.SetFillPercent((float)newHealth / _character.Damageable.MaxHealth);
        }

        private void OnNewHostileRegistered(HostileEntry entry) {
            if(entry.Hostile != PlayerCharacter.Instance) {
                return;
            }
            _playerHostileEntry = entry;
            _awarenessLevel.gameObject.SetActive(true);
        }

        private void OnRegisteredHostilesUpdated(int count) {
            if (!ShouldShowAwarenessMeter(count)) {
                DisableAwarenessMeter();
                return;
            }
            _awarenessLevel.SetFillPercent(Mathf.Clamp(_playerHostileEntry.DetectionValue, 0f, 1f));
        }

        private void OnCurrentTargetUpdated() {
            // attempt to retrieve player's entry
            _npcTargetManager.RegisteredHostiles.TryGetValue(PlayerCharacter.Instance, out _playerHostileEntry);
            if(ShouldShowAwarenessMeter(_npcTargetManager.RegisteredHostiles.Count)) {
                _awarenessLevel.gameObject.SetActive(true);
                _awarenessLevel.SetFillPercent(Mathf.Clamp(_playerHostileEntry.DetectionValue, 0f, 1f));
                return;
            }
            // disable awareness meter
            DisableAwarenessMeter();
        }

        private bool ShouldShowAwarenessMeter(int count) {
            return count > 0 && _npcTargetManager.CurrentTarget == null && _playerHostileEntry != null
                && _npcTargetManager.RegisteredHostiles.ContainsKey(_playerHostileEntry.Hostile);
        }

        private void DisableAwarenessMeter() {
            _playerHostileEntry = null;
            _awarenessLevel.gameObject.SetActive(false);
        }
    }
}
