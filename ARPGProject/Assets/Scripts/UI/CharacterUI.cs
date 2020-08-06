using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class CharacterUI : MonoBehaviour {

        [SerializeField] private FillBar _healthBar;
        [SerializeField] private FillBar _awarenessLevel;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;

        private ICharacter _character;
        private NPCTargetManager _npcTargetManager;
        private HostileEntry _playerHostileEntry;
        private Vector3 _offset;

        public event Action<ICharacter, CharacterUI> OnDisableAwarenessMeter;

        private void Update() {
            // set the position
            Vector3 targetPosition = _character.MoveController.Body.position + _offset;
            Vector2 newPosition = UIUtility.WorldToScreenSpace(
                _canvas,
                CameraController.Instance.MainCamera,
                targetPosition);
            _rectTransform.anchoredPosition = newPosition;
        }

        public bool Initialize(ICharacter character, Canvas canvas) {
            _npcTargetManager = character.TargetManager as NPCTargetManager;
            if(_npcTargetManager == null) {
                CustomLogger.Error(this.name, $"Character {character.MoveController.Body.name} did not have component of type {nameof(NPCTargetManager)}");
                return false;
            }
            // set fields
            _canvas = canvas;
            _character = character;
            _offset = Vector3.up * _character.MoveController.Height;
            // listen for events
            _character.Damageable.OnCurrentHealthChanged += OnCurrentHealthChanged;
            _npcTargetManager.OnRegisteredHostilesUpdated += OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet += OnCurrentTargetUpdated;
            // perform initial events
            TryEnableAwarenessMeter();
            OnCurrentHealthChanged(_character.Damageable.Health);
            return true;
        }

        private void TryEnableAwarenessMeter() {
            if (_npcTargetManager.RegisteredHostiles.TryGetValue(PlayerCharacter.Instance, out HostileEntry entry)) {
                EnableAwarenessMeter(entry);
            } else {
                DisableAwarenessMeter();
            }
        }

        public void Dispose() {
            _character.Damageable.OnCurrentHealthChanged -= OnCurrentHealthChanged;
            _npcTargetManager.OnRegisteredHostilesUpdated -= OnRegisteredHostilesUpdated;
            _npcTargetManager.OnCurrentTargetSet -= OnCurrentTargetUpdated;
        }

        private void OnCurrentHealthChanged(int newHealth) {
            _healthBar.SetFillPercent((float)newHealth / _character.Damageable.MaxHealth);
        }


        private void OnRegisteredHostilesUpdated(int count) {
            if (!ShouldShowAwarenessMeter(count)) {
                DisableAwarenessMeter();
                return;
            }
            _awarenessLevel.SetFillPercent(Mathf.Clamp(_playerHostileEntry.DetectionValue, 0f, 1f));
        }

        private void OnCurrentTargetUpdated() {
            Debug.Log("Current target updated");
            // attempt to retrieve player's entry
            _npcTargetManager.RegisteredHostiles.TryGetValue(PlayerCharacter.Instance, out _playerHostileEntry);
            if(ShouldShowAwarenessMeter(_npcTargetManager.RegisteredHostiles.Count)) {
                EnableAwarenessMeter(_playerHostileEntry);
                return;
            }
            // disable awareness meter
            DisableAwarenessMeter();
        }

        private bool ShouldShowAwarenessMeter(int count) {
            return count > 0 && _npcTargetManager.CurrentTarget == null && _playerHostileEntry != null
                && _npcTargetManager.RegisteredHostiles.ContainsKey(_playerHostileEntry.Hostile);
        }

        private void EnableAwarenessMeter(HostileEntry entry) {
            _playerHostileEntry = entry;
            _awarenessLevel.gameObject.SetActive(true);
            _awarenessLevel.SetFillPercent(Mathf.Clamp(_playerHostileEntry.DetectionValue, 0f, 1f));
        }

        private void DisableAwarenessMeter() {
            _playerHostileEntry = null;
            _awarenessLevel.gameObject.SetActive(false);
            OnDisableAwarenessMeter?.Invoke(_character, this);
        }
    }
}
