using System;

namespace Bebis {
    public interface IDamageable {
        int Health { get; }
        int MaxHealth { get; }

        event Action<HitEventInfo> OnHit;
        event Action OnDefeated;
    }
}