using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HurtboxControllerV2 : MonoBehaviour, ICharacterComponent
    {
        public IReadOnlyDictionary<string, HurtboxV2> Hurtboxes => _hurtBoxes;

        public event Action<HitEventInfoV2> OnHit;

        protected ICharacterV2 _character;
        [SerializeField] protected LayerMask _hurtBoxLayers;
        [SerializeField] protected List<HurtboxV2> _hurtBoxObjs = new List<HurtboxV2>();
        protected readonly Dictionary<string, HurtboxV2> _hurtBoxes = new Dictionary<string, HurtboxV2>();

        protected bool _receivedHit;
        protected Action<ICharacterV2> _onInteractionSuccess;
        protected Action<ICharacterV2> _onInteractionFailed;

        public void Initialize(ICharacterV2 character) {
            _character = character;
            _hurtBoxes.Clear();
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxes.Add(_hurtBoxObjs[i].name, _hurtBoxObjs[i]);
                _hurtBoxObjs[i].Initialize(_character);
                _hurtBoxObjs[i].OnInteraction += OnHurtboxInteraction;
            }
        }

        private void LateUpdate() {
            if (_receivedHit) {
                ProcessHit();
            }
        }

        private void OnDestroy() {
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxObjs[i].OnInteraction -= OnHurtboxInteraction;
            }
        }

        protected virtual void OnHurtboxInteraction(HurtboxInteractionV2 interaction) {
            // TODO: take into account hitbox state
            _receivedHit = true;
            _onInteractionSuccess = interaction.OnInteractionSuccess;
            _onInteractionFailed = interaction.OnInteractionFail;
        }

        private void ProcessHit() {
            // Debug.LogError("Received hit event!");
            _onInteractionSuccess?.Invoke(_character);
            _receivedHit = false;
            _onInteractionSuccess = null;
            _onInteractionFailed = null;
        }
    }
}
