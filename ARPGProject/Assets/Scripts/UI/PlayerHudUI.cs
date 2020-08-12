﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class PlayerHudUI : MonoBehaviour {

        [SerializeField] private GameObject _hudParent;
        [SerializeField] private FillBar _playerHealthBar;
        [SerializeField] private Text _healthTextDisplay;

        private void Start() {
            PlayerCharacter.Instance.Damageable.OnCurrentHealthChanged += OnCurrentHealthChanged;
            PlayerCharacter.Instance.Damageable.OnMaxHealthChanged += OnMaxHealthChanged;
            OnCurrentHealthChanged(PlayerCharacter.Instance.Damageable.Health);
            OnMaxHealthChanged(PlayerCharacter.Instance.Damageable.MaxHealth);
        }

        public void SetHudEnabled(bool enabled) {
            _hudParent.SetActive(enabled);
        }

        private void OnCurrentHealthChanged(int newHealth) {
            _playerHealthBar.SetFillPercent(newHealth / PlayerCharacter.Instance.Damageable.MaxHealth);
            _healthTextDisplay.text = $"{newHealth} / {PlayerCharacter.Instance.Damageable.MaxHealth}";
        }

        private void OnMaxHealthChanged(int newMaxHealth) {
            _healthTextDisplay.text = $"{PlayerCharacter.Instance.Damageable.Health} / {newMaxHealth}";
        }
    }
}