using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Winston;

namespace Bebis {
    public class PlayerTargetsUIManager : MonoBehaviour, IUIDisplay {

        private const string CharacterUIPrefabId = "CharacterUI";

        [SerializeField] private int _poolSize;

        [SerializeField] private Transform _characterUILayer;
        [SerializeField] private Canvas _displayCanvas;

        private readonly List<CharacterUI> _characterUIs = new List<CharacterUI>();
        private readonly Dictionary<ICharacter, CharacterUI> _activeCharacterUIs = new Dictionary<ICharacter, CharacterUI>();

        public void Initialize(UIDisplayInitializationData initializationData) {
            RegisterCharacterPrefabs();

            PlayerTargetManager.Instance.OnRegisterCharacter += OnCharacterRegistered;
            PlayerTargetManager.Instance.OnDeregisterCharacter += OnCharacterDeregistered;
        }

        public void OnHide() {
            
        }

        public void Destroy() {
            
        }

        // called by NPCs when they detect the player
        public void RegisterPlayer(ICharacter character) {
            if (_activeCharacterUIs.ContainsKey(character)) {
                return;
            }
            OnCharacterRegistered(character);
        }

        private void RegisterCharacterPrefabs() {
            ManagerMaster.PooledObjectManager.RegisterPooledObject(CharacterUIPrefabId, _poolSize);
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
            CharacterUIInitData initData = new CharacterUIInitData {
                Character = character,
                Canvas = _displayCanvas
            };
            // if failed to initialize
            if(!characterUI.Initialize(initData)) {
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
