using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Winston;

namespace Bebis {
    public class PlayerHudUI : MonoBehaviour, IUIDisplay {

        [SerializeField] private GameObject _hudParent;
        [SerializeField] private FillBar _playerHealthBar;
        [SerializeField] private Text _healthTextDisplay;
        [SerializeField] private PlayerSkillsView _playerSkillsView;

        private void SetupHealthDisplay() {
            PlayerCharacter.Instance.Damageable.OnCurrentHealthChanged += OnCurrentHealthChanged;
            PlayerCharacter.Instance.Damageable.OnMaxHealthChanged += OnMaxHealthChanged;

            OnCurrentHealthChanged(PlayerCharacter.Instance.Damageable.Health);
            OnMaxHealthChanged(PlayerCharacter.Instance.Damageable.MaxHealth);
        }

        private void SetupSkillsDisplay(PlayerActionInfoProvider playerActionInfoProvider) {
            _playerSkillsView.Initialize(playerActionInfoProvider);
        }

        public void SetHudEnabled(bool enabled) {
            _hudParent.SetActive(enabled);
        }

        private void OnCurrentHealthChanged(int newHealth) {
            _playerHealthBar.SetFillPercent((float)newHealth / PlayerCharacter.Instance.Damageable.MaxHealth);
            _healthTextDisplay.text = $"{newHealth} / {PlayerCharacter.Instance.Damageable.MaxHealth}";
        }

        private void OnMaxHealthChanged(int newMaxHealth) {
            _healthTextDisplay.text = $"{PlayerCharacter.Instance.Damageable.Health} / {newMaxHealth}";
        }

        public void Initialize(UIDisplayInitializationData initializationData) {
            PlayerHudInitializationData playerHudInitData = initializationData as PlayerHudInitializationData;
            if(playerHudInitData == null) {
                return;
            }
            SetupHealthDisplay();
            SetupSkillsDisplay(playerHudInitData.PlayerActionInfoProvider);
        }

        public void OnHide() {

        }

        public void Destroy() {

        }
    }

    public class PlayerHudInitializationData : UIDisplayInitializationData
    {
        public PlayerActionInfoProvider PlayerActionInfoProvider;
    }
}
