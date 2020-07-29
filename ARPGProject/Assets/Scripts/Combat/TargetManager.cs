using System;

namespace Bebis {
    public interface ITargetManager {

        ICharacter CurrentTarget { get; }
        event Action OnCurrentTargetSet;

        void OverrideCurrentTarget(ICharacter character);
        void ClearCurrentTarget();
    }
}
