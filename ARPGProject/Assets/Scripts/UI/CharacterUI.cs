using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class CharacterUI : MonoBehaviour, UIObject {

        [SerializeField] private FillBar _healthBar;
        [SerializeField] private FillBar _awarenessLevel;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private GameObject _uiParent; // controls display for all UI objects
        [SerializeField] private Canvas _canvas;

        private ICharacter _character;
        private NPCTargetManager _npcTargetManager;
        private HostileEntry _playerHostileEntry;
        private Vector3 _offset;
        private Vector3 _worldSpacePosition => _character.MoveController.Body.position + _offset;

        public event Action<ICharacter, CharacterUI> OnDisableAwarenessMeter;

        private void Update() {
            SetUIParentActive(_worldSpacePosition);
            SetUIPosition();
        }

        public bool Initialize(UIObjectInitData initData) {
            CharacterUIInitData characterUIInitData = initData as CharacterUIInitData;
            if(characterUIInitData == null) {
                CustomLogger.Error(this.name, $"Init data was not of type {nameof(CharacterUIInitData)}");
                return false;
            }
            ICharacter character = characterUIInitData.Character;
            Canvas canvas = characterUIInitData.Canvas;
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

        private void SetUIPosition() {
            // set the position
            Vector2 newPosition = UIUtility.WorldToScreenSpace(
                _canvas,
                CameraController.Instance.MainCamera,
                _worldSpacePosition);
            _rectTransform.anchoredPosition = newPosition;
        }

        private void SetUIParentActive(Vector3 targetPosition) {
            _uiParent.SetActive(IsInCameraView(targetPosition));
        }

        private static bool IsInCameraView(Vector3 targetPosition) {
            Vector3 viewPos = CameraController.Instance.MainCamera.WorldToViewportPoint(targetPosition);
            return viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f && viewPos.z >= 0f;
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

    public class CharacterUIInitData : UIObjectInitData {
        public ICharacter Character;
        public Canvas Canvas;
    }
}
