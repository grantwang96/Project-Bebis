using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class TempCharacterStatsDisplay : MonoBehaviour {

        [SerializeField] private GameObject _characterGameObject;
        private ICharacter _character;

        [SerializeField] private Text _strengthText;
        [SerializeField] private Text _agilityText;
        [SerializeField] private Text _fortitudeText;
        [SerializeField] private Text _intelligenceText;
        [SerializeField] private Text _wisdomText;
        [SerializeField] private Text _charmText;

        // Start is called before the first frame update
        void Start() {
            _character = _characterGameObject.GetComponent<ICharacter>();
            _character.CharacterStatManager.OnFinalStatsUpdated += OnCharacterStatsUpdated;
            OnCharacterStatsUpdated(_character.CharacterStatManager.FinalStats);
        }

        private void OnDestroy() {
            _character.CharacterStatManager.OnFinalStatsUpdated -= OnCharacterStatsUpdated;
        }

        private void OnCharacterStatsUpdated(CharacterStats stats) {
            _strengthText.text = $"Strength: {stats.Strength}";
            _agilityText.text = $"Agility: {stats.Agility}";
            _fortitudeText.text = $"Fortitude: {stats.Fortitude}";
            _intelligenceText.text = $"Intelligence: {stats.Intelligence}";
            _wisdomText.text = $"Wisdom: {stats.Wisdom}";
            _charmText.text = $"Charm: {stats.Charm}";
        }
    }
}
