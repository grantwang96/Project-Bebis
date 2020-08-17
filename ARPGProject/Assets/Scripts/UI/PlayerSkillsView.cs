using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class PlayerSkillsView : MonoBehaviour, UIObject {

        [SerializeField] private GameObject _uiParent;
        [SerializeField] private Image _skill1Image;
        [SerializeField] private Text _skill1Text;
        [SerializeField] private Image _skill2Image;
        [SerializeField] private Text _skill2Text;
        [SerializeField] private Image _skill3Image;
        [SerializeField] private Text _skill3Text;
        [SerializeField] private Image _skill4Image;
        [SerializeField] private Text _skill4Text;

        private PlayerActionController _playerActionController;

        public bool Initialize(UIInitData data) {
            IActionController actionController = PlayerCharacter.Instance.ActionController;
            _playerActionController = actionController as PlayerActionController;
            if(_playerActionController == null) {
                CustomLogger.Error(nameof(PlayerSkillsView), $"Could not set {nameof(PlayerActionController)}!");
                return false;
            }
            _playerActionController.OnCurrentActionSetUpdated += OnCurrentActionSetUpdated;
            OnCurrentActionSetUpdated();
            return true;
        }

        public void Dispose() {
            _playerActionController.OnCurrentActionSetUpdated -= OnCurrentActionSetUpdated;
        }

        private void OnCurrentActionSetUpdated() {
            IPlayerGameplayActionSet _currentActionSet = _playerActionController.CurrentActionSet;
            // TODO: set the images
            SetSkillEntry(_currentActionSet.Btn1Skill, _skill1Text, _skill1Image);
            SetSkillEntry(_currentActionSet.Btn2Skill, _skill2Text, _skill2Image);
            SetSkillEntry(_currentActionSet.Btn3Skill, _skill3Text, _skill3Image);
            SetSkillEntry(_currentActionSet.Btn4Skill, _skill4Text, _skill4Image);
        }

        private void SetSkillEntry(CharacterActionData data, Text text, Image icon) {
            if(data == null) {
                // TODO: set the image
                text.text = "Empty";
                return;
            }
            text.text = data.ActionName;
            // TODO: set the image
        }
    }
}
