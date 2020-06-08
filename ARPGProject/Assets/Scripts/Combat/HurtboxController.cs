using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class HurtboxController : MonoBehaviour {

        [SerializeField] private GameObject _characterGO;
        [SerializeField] private List<Hurtbox> _hurtBoxObjs = new List<Hurtbox>();
        private ICharacter _character;
        private readonly Dictionary<string, Hurtbox> _hurtBoxes = new Dictionary<string, Hurtbox>();

        private bool _receivedHit;
        private readonly List<Hitbox> _defendedHitboxes = new List<Hitbox>();
        private Action<ICharacter> _onHitAction;

        public IReadOnlyDictionary<string, Hurtbox> Hurtboxes => _hurtBoxes;
        public HurtBoxState HitHurtboxState { get; private set; }
        public event Action<HitEventInfo> OnHit;

        private void OnEnable() {
            _hurtBoxes.Clear();
            _character = _characterGO.GetComponent<ICharacter>();
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxes.Add(_hurtBoxObjs[i].name, _hurtBoxObjs[i]);
                _hurtBoxObjs[i].Initialize(_character);
                _hurtBoxObjs[i].OnHit += OnHurtboxHit;
                _hurtBoxObjs[i].OnDefend += OnHurtboxDefend;
            }
        }

        private void OnDisable() {
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxObjs[i].OnHit -= OnHurtboxHit;
                _hurtBoxObjs[i].OnDefend -= OnHurtboxDefend;
            }
            _hurtBoxes.Clear();
        }

        private void LateUpdate() {
            if (_receivedHit) {
                ProcessHit();
            }
        }

        private void OnDestroy() {
            for(int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxObjs[i].OnHit -= OnHurtboxHit;
            }
        }

        private void OnHurtboxHit(Hitbox hitbox, Action<ICharacter> onHitAction) {
            // ignore hitbox if it was defended against
            if (_defendedHitboxes.Contains(hitbox)) {
                return;
            }
            // TODO: take into account hitbox state
            _receivedHit = true;
            // TODO: have comparison take into account various hits on the same frame and have one take priority
            HitHurtboxState = HurtBoxState.Normal;
            _onHitAction = onHitAction;
        }

        private void OnHurtboxDefend(Hitbox hitbox, Action<ICharacter> onHitAction) {
            // add this hitbox to list that cannot process hit
            if (!_defendedHitboxes.Contains(hitbox)) {
                _defendedHitboxes.Add(hitbox);
                hitbox.OnHitboxInitialized += OnRegisteredHitboxInitialized;
            }
            // for now, allow hitbox to process how to handle defending hitbox
            _receivedHit = true;
            HitHurtboxState = HurtBoxState.Defending;
            _onHitAction = onHitAction;
        }

        private void ProcessHit() {
            _receivedHit = false;
            _onHitAction?.Invoke(_character);
        }

        private void OnRegisteredHitboxInitialized(Hitbox hitbox) {
            // remove hitbox when it has been reset
            _defendedHitboxes.Remove(hitbox);
        }
    }
}
