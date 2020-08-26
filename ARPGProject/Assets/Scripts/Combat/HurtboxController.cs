using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public abstract class HurtboxController : MonoBehaviour {

        protected ICharacter _character;
        [SerializeField] protected LayerMask _hurtBoxLayers;
        [SerializeField] protected List<Hurtbox> _hurtBoxObjs = new List<Hurtbox>();
        protected readonly Dictionary<string, Hurtbox> _hurtBoxes = new Dictionary<string, Hurtbox>();

        protected bool _receivedHit;
        protected readonly List<Hitbox> _registeredHitboxes = new List<Hitbox>();
        protected Action<ICharacter> _onInteractionSuccess;
        protected Action<ICharacter> _onInteractionInterrupted;

        public IReadOnlyDictionary<string, Hurtbox> Hurtboxes => _hurtBoxes;
        public IReadOnlyList<Hitbox> RegisteredHitboxes => _registeredHitboxes;
        public HurtBoxState HitHurtboxState { get; private set; }
        public event Action<HitEventInfo> OnHit;

        protected virtual void OnEnable() {
            _hurtBoxes.Clear();
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxes.Add(_hurtBoxObjs[i].name, _hurtBoxObjs[i]);
                _hurtBoxObjs[i].Initialize(_character);
                _hurtBoxObjs[i].OnHit += OnHurtboxHit;
                _hurtBoxObjs[i].OnDefend += OnHurtboxDefend;
            }
        }

        protected virtual void OnDisable() {
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
            for (int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxObjs[i].OnHit -= OnHurtboxHit;
                _hurtBoxObjs[i].OnDefend -= OnHurtboxDefend;
            }
        }

        protected virtual void OnHurtboxHit(HurtboxInteraction interaction) {
            // ignore hitbox if it was defended against
            if (_registeredHitboxes.Contains(interaction.Hitbox)) {
                return;
            }
            if(HitDefendingBox(interaction.OtherCharacter, interaction.Hurtbox, interaction.Hitbox)) {
                OnHurtboxDefend(interaction);
                return;
            }
            _registeredHitboxes.Add(interaction.Hitbox);
            interaction.Hitbox.OnHitboxInitialized += OnRegisteredHitboxInitialized;
            // TODO: take into account hitbox state
            _receivedHit = true;
            // if there is already a registered hit, override it
            if(_onInteractionSuccess != null) {
                _onInteractionInterrupted?.Invoke(_character);
            }
            HitHurtboxState = HurtBoxState.Normal;
            _onInteractionSuccess = interaction.OnInteractionSuccess;
            _onInteractionInterrupted = interaction.OnInteractionFail;
        }

        protected bool HitDefendingBox(ICharacter otherCharacter, Hurtbox targetHurtbox, Hitbox hitbox) {
            // perform raycast between attacker and intended hurtbox
            Vector3 start = otherCharacter.MoveController.Center.position;
            Vector3 direction = targetHurtbox.transform.position - start;
            float distance = Vector3.Distance(transform.position, start);
            RaycastHit[] hitInfos = Physics.RaycastAll(start, direction.normalized, distance, _hurtBoxLayers);
            for (int i = 0; i < hitInfos.Length; i++) {
                // if there is a defending hurtbox in this path, return true
                RaycastHit hitInfo = hitInfos[i];
                Hurtbox hurtBox = hitInfo.collider.GetComponent<Hurtbox>();
                if (hurtBox != null && _hurtBoxObjs.Contains(hurtBox) && hurtBox.HurtBoxState != HurtBoxState.Normal) {
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnHurtboxDefend(HurtboxInteraction interaction) {
            // add this hitbox to list that cannot process hit
            if (!_registeredHitboxes.Contains(interaction.Hitbox)) {
                return;
            }
            ICharacter otherCharacter = interaction.OtherCharacter;
            // override shield direction
            if(_character != null) {
                Vector3 direction = (otherCharacter.MoveController.Body.position - _character.MoveController.Body.position).normalized;
                _character.MoveController.OverrideRotation(direction);
            }
            // register that this hitbox has been hit
            _registeredHitboxes.Add(interaction.Hitbox);
            interaction.Hitbox.OnHitboxInitialized += OnRegisteredHitboxInitialized;
            _receivedHit = true;
            if (_onInteractionSuccess != null) {
                _onInteractionInterrupted?.Invoke(_character);
            }
            // for now, allow hitbox to process how to handle defending hurtbox
            HitHurtboxState = HurtBoxState.Defending;
            _onInteractionSuccess = interaction.OnInteractionSuccess;
            _onInteractionInterrupted = interaction.OnInteractionFail;
        }

        protected void ProcessHit() {
            _receivedHit = false;
            _onInteractionSuccess?.Invoke(_character);
            _onInteractionSuccess = null;
            _onInteractionInterrupted = null;
        }

        protected void OnRegisteredHitboxInitialized(Hitbox hitbox) {
            // remove hitbox when it has been reset
            _registeredHitboxes.Remove(hitbox);
        }
    }
}
