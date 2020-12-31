using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HurtboxControllerV2 : MonoBehaviour, ICharacterComponent
    {
        public IReadOnlyDictionary<string, HurtboxV2> Hurtboxes => _hurtBoxes;
        public IReadOnlyList<HitboxV2> RegisteredHitboxes => _registeredHitboxes;

        public event Action<HitEventInfoV2> OnHit;

        protected ICharacterV2 _character;
        [SerializeField] protected LayerMask _hurtBoxLayers;
        [SerializeField] protected List<HurtboxV2> _hurtBoxObjs = new List<HurtboxV2>();
        protected readonly Dictionary<string, HurtboxV2> _hurtBoxes = new Dictionary<string, HurtboxV2>();

        protected bool _receivedHit;
        protected readonly List<HitboxV2> _registeredHitboxes = new List<HitboxV2>();
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
            // ignore hitbox if it was defended against
            if (_registeredHitboxes.Contains(interaction.Hitbox)) {
                return;
            }
            _registeredHitboxes.Add(interaction.Hitbox);
            // TODO: take into account hitbox state
            _receivedHit = true;
            // if there is already a registered hit, override it
            if (_onInteractionSuccess != null) {
                _onInteractionFailed?.Invoke(_character);
            }
            _onInteractionSuccess = interaction.OnInteractionSuccess;
            _onInteractionFailed = interaction.OnInteractionFail;
        }

        private void ProcessHit() {
            _receivedHit = false;
            _onInteractionSuccess = null;
            _onInteractionFailed = null;
            _registeredHitboxes.Clear();
        }
    }
}
