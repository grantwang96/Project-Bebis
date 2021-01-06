using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public abstract class TriggerActionData : CharacterActionData {

        [SerializeField] protected AnimationData _initialActionAnimationData;
        [SerializeField] protected AnimationData _onTriggerSuccess;
        [SerializeField] protected AnimationData _onTriggerFailed;
        [SerializeField] protected AnimationData _onTriggerInterrupted;
        [SerializeField] protected List<TriggerBoxDataEntry> _triggerBoxDatas = new List<TriggerBoxDataEntry>();

        [SerializeField] protected bool _affectNormalColliders;
        [SerializeField] protected bool _beatShields;

        public AnimationData InitialActionAnimationData => _initialActionAnimationData;
        public AnimationData OnTriggerSuccess => _onTriggerSuccess;
        public AnimationData OnTriggerFailed => _onTriggerFailed;
        public AnimationData OnTriggerInterrupted => _onTriggerInterrupted;
        public IReadOnlyList<TriggerBoxDataEntry> TriggerBoxDatas => _triggerBoxDatas;
        public bool AffectNormalColliders => _affectNormalColliders;
        public bool BeatShields => _beatShields;
    }

    [System.Serializable]
    public class TriggerBoxDataEntry {
        [SerializeField] private string _id;
        [SerializeField] private TriggerBoxData _triggerBoxData;

        public string Id => _id;
        public TriggerBoxData TriggerBoxData => _triggerBoxData;
    }

    [System.Serializable]
    public class TriggerBoxData {

    }

    public abstract class TriggerActionState : CharacterActionState {

        private const string FailTriggerId = "TriggerFailed";
        private const string TriggerHitId = "TriggerHit";

        protected TriggerActionData _triggerActionData;
        protected readonly Dictionary<string, TriggerBoxData> _triggerBoxDatas = new Dictionary<string, TriggerBoxData>();
        protected ICharacter _target;
        protected bool _triggered;

        public TriggerActionState(TriggerActionData data, ICharacter character) : base(data, character) {
            _triggerActionData = data;
        }

        // children will call this depending on which interaction triggers the action
        protected virtual void PerformTriggerAction() {
            SetTriggerBoxInfos();
            _character.AnimationController.OnAnimationMessageSent += OnAnimationMessageSent;
            _character.AnimationController.OnAnimationStateUpdated += OnAnimationStateUpdated;
            _character.AnimationController.UpdateAnimationState(_triggerActionData.InitialActionAnimationData);
        }

        protected void SetTriggerBoxInfos() {
            _triggerBoxDatas.Clear();
            for(int i = 0; i < _triggerActionData.TriggerBoxDatas.Count; i++) {
                // save the entry to the dictionary
                TriggerBoxDataEntry triggerBoxDataEntry = _triggerActionData.TriggerBoxDatas[i];
                _triggerBoxDatas.Add(triggerBoxDataEntry.Id, triggerBoxDataEntry.TriggerBoxData);
                // create a hitbox info and initialize hitbox by id
                CombatHitboxInfo3D info = new CombatHitboxInfo3D(OnHitboxTriggered);
                _character.HitboxController.SetHitboxInfo(triggerBoxDataEntry.Id, info);
            }
        }

        protected virtual void OnHitboxTriggered(Hitbox hitBox, Collider collider) {
            if (_triggered) {
                return;
            }
            // if failed to retrieve hitbox data
            if (!_triggerBoxDatas.TryGetValue(hitBox.name, out TriggerBoxData triggerBoxData)) {
                CustomLogger.Warn(nameof(AttackActionData3D), $"Could not retrieve trigger box data for hitbox {hitBox.name}");
                return;
            }
            Hurtbox hurtBox = collider.GetComponent<Hurtbox>();
            // if there is no hurtbox and this attack can affect normal colliders
            if(hurtBox != null) {
                HitHurtbox(hitBox, hurtBox, triggerBoxData);
                return;
            }
            if (_triggerActionData.AffectNormalColliders) {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                HitDamageable(damageable, collider, triggerBoxData);
            }
        }

        protected bool ShouldTrigger(HurtBoxState hurtBoxState) {
            if (_triggerActionData.BeatShields) {
                return hurtBoxState == HurtBoxState.Normal || hurtBoxState == HurtBoxState.Defending;
            }
            return hurtBoxState == HurtBoxState.Normal;
        }

        protected virtual void HitHurtbox(Hitbox hitBox, Hurtbox hurtBox, TriggerBoxData triggerBoxData) {
            // determine if this hitbox can trigger
            if (!ShouldTrigger(hurtBox.HurtBoxState)) {
                return;
            }
            // ensure this isn't the character's own hitbox
            List<Hurtbox> characterHurtBoxes = new List<Hurtbox>(_character.HurtboxController.Hurtboxes.Values);
            if (characterHurtBoxes.Contains(hurtBox)) {
                return;
            }
            // tell the hurtbox that it was hit
            hurtBox.SendHitEvent(_character, hitBox, TriggerSuccess, TriggerInterrupted);
        }

        protected virtual void HitDamageable(IDamageable damageable, Collider collider, TriggerBoxData triggerBoxData) {
            _character.AnimationController.UpdateAnimationState(_triggerActionData.OnTriggerSuccess);
            _triggered = true;
        }
        
        protected virtual void OnAnimationMessageSent(string message) {
            // if the original animation failed to trigger
            if (message == FailTriggerId) {
                TriggerMiss();
            // if the success animation hits the effect point
            } else if(message == TriggerHitId) {
                OnTriggerEffect();
            }
        }

        protected void OnAnimationStateUpdated(AnimationState animationState) {
            switch (animationState) {
                case AnimationState.Started:
                    UpdateActionStatus(ActionStatus.Started);
                    break;
                case AnimationState.InProgress:
                    UpdateActionStatus(ActionStatus.InProgress);
                    break;
                case AnimationState.CanTransition:
                    UpdateActionStatus(ActionStatus.CanTransition);
                    break;
                case AnimationState.Completed:
                    UpdateActionStatus(ActionStatus.Completed);
                    break;
            }
        }

        public override void Clear() {
            base.Clear();
            _character.AnimationController.OnAnimationMessageSent -= OnAnimationMessageSent;
            _character.AnimationController.OnAnimationStateUpdated -= OnAnimationStateUpdated;
        }

        // this trigger action has been successfully...triggered...
        protected virtual void TriggerSuccess(ICharacter target) {
            // check if target STILL can be triggered
            if (!ShouldTrigger(target.HurtboxController.HitHurtboxState)) {
                return;
            }
            _target = target;
            // apply other effects to the character (such as holding)
            _character.AnimationController.UpdateAnimationState(_triggerActionData.OnTriggerSuccess);
            _triggered = true;
        }
        
        // this trigger action was successful, but interrupted
        protected virtual void TriggerInterrupted(ICharacter target) {
            _character.AnimationController.UpdateAnimationState(_triggerActionData.OnTriggerInterrupted);
        }

        // the actual effect of the action (take damage, release held character, etc.)
        protected abstract void OnTriggerEffect();
        
        // this trigger action missed
        protected virtual void TriggerMiss() {
            _character.AnimationController.UpdateAnimationState(_triggerActionData.OnTriggerFailed);
        }
    }
}
