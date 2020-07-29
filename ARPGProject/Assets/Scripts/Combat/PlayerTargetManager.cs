using System;

namespace Bebis {
    public class PlayerTargetManager : CharacterComponent, ITargetManager {

        public ICharacter CurrentTarget { get; private set; }
        public event Action OnCurrentTargetSet;

        public void OverrideCurrentTarget(ICharacter character) {
            if(CurrentTarget != null) {
                CurrentTarget.Damageable.OnDefeated -= OnCurrentTargetDefeated;
            }
            CurrentTarget = character;
            CurrentTarget.Damageable.OnDefeated += OnCurrentTargetDefeated;
        }

        public void ClearCurrentTarget() {
            CurrentTarget = null;
            CurrentTarget.Damageable.OnDefeated -= OnCurrentTargetDefeated;
        }

        private void OnCurrentTargetDefeated() {
            ClearCurrentTarget();
        }
    }
}
