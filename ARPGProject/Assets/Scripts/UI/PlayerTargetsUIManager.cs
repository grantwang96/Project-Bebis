using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class PlayerTargetsUIManager : MonoBehaviour {

        public static PlayerTargetsUIManager Instance { get; private set; }

        [SerializeField] private CharacterUI _characterPrefab; // TODO: replace this with asset management system
        [SerializeField] private int _poolSize;

        [SerializeField] private Transform _characterUILayer;
        [SerializeField] private Canvas _displayCanvas;

        private readonly List<CharacterUI> _characterUIs = new List<CharacterUI>();
        private readonly Dictionary<ICharacter, CharacterUI> _activeCharacterUIs = new Dictionary<ICharacter, CharacterUI>();

        private void Awake() {
            Instance = this;
        }

        // Start is called before the first frame update
        private void Start() {
            RegisterCharacterPrefabs();

            PlayerTargetManager.Instance.OnRegisterCharacter += OnCharacterRegistered;
            PlayerTargetManager.Instance.OnDeregisterCharacter += OnCharacterDeregistered;
        }

        // called by NPCs when they detect the player
        public void RegisterPlayer(ICharacter character) {
            if (_activeCharacterUIs.ContainsKey(character)) {
                return;
            }
            OnCharacterRegistered(character);
        }

        private void RegisterCharacterPrefabs() {
            for(int i = 0; i < _poolSize; i++) {
                CharacterUI ui = Instantiate(_characterPrefab, _characterUILayer);
                ui.gameObject.SetActive(false);
                _characterUIs.Add(ui);
            }
        }

        private void OnCharacterRegistered(ICharacter character) {
            if (_activeCharacterUIs.ContainsKey(character)) {
                return;
            }
            if(_characterUIs.Count == 0) {
                CustomLogger.Warn(nameof(PlayerTargetsUIManager), $"Reached max ui objects ({_poolSize}) for {nameof(PlayerTargetsUIManager)}!");
                return;
            }
            CharacterUI characterUI = _characterUIs[0];
            // if failed to initialize
            if(!characterUI.Initialize(character, _displayCanvas)) {
                CustomLogger.Error(nameof(PlayerTargetsUIManager), $"Failed to initialize {nameof(CharacterUI)}!");
                return;
            }
            characterUI.gameObject.SetActive(true);
            characterUI.OnDisableAwarenessMeter += OnAwarenessMeterDisabled;
            _characterUIs.RemoveAt(0);
            _activeCharacterUIs.Add(character, characterUI);
        }

        private void OnCharacterDeregistered(ICharacter character) {
            if(!_activeCharacterUIs.TryGetValue(character, out CharacterUI characterUI)) {
                CustomLogger.Error(nameof(PlayerTargetsUIManager), $"Could not retrieve character entry with key {character.MoveController.Body.name}!");
                return;
            }
            characterUI.Dispose();
            characterUI.gameObject.SetActive(false);
            _activeCharacterUIs.Remove(character);
            _characterUIs.Add(characterUI);
        }

        // if the awareness meter gets disabled
        private void OnAwarenessMeterDisabled(ICharacter character, CharacterUI characterUI) {
            // if the character is focused on the player, keep the UI up
            if(character.TargetManager.CurrentTarget == PlayerCharacter.Instance) {
                return;
            }
            // TODO: allow for some time before this UI object gets removed. For now insta-remove
            OnCharacterDeregistered(character);
        }
    }
}
