using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public class PlayerTargetManager : CharacterComponent, ITargetManager {

        public static PlayerTargetManager Instance { get; private set; }

        // list of "combatants" the player knows about
        private readonly List<ICharacter> _registeredCharacters = new List<ICharacter>();

        public ICharacter CurrentTarget { get; private set; }
        public event Action OnCurrentTargetSet;

        public event Action<ICharacter> OnRegisterCharacter;
        public event Action<ICharacter> OnDeregisterCharacter;

        protected override void Awake() {
            base.Awake();
            Instance = this;
        }

        public void OverrideCurrentTarget(ICharacter character) {
            if(CurrentTarget != null) {
                CurrentTarget.Damageable.OnDefeated -= OnCurrentTargetDefeated;
            }
            CurrentTarget = character;
            CurrentTarget.Damageable.OnDefeated += OnCurrentTargetDefeated;
            if (_registeredCharacters.Contains(character)) {
                return;
            }
            _registeredCharacters.Add(character);
            OnRegisterCharacter?.Invoke(character);
        }

        public void ClearCurrentTarget() {
            CurrentTarget.Damageable.OnDefeated -= OnCurrentTargetDefeated;
            _registeredCharacters.Remove(CurrentTarget);
            OnDeregisterCharacter?.Invoke(CurrentTarget);
            CurrentTarget = null;
        }

        private void OnCurrentTargetDefeated() {
            ClearCurrentTarget();
        }
    }
}
