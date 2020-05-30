using System;

namespace Bebis {
    public interface IDamageable {
        int Health { get; }
        int MaxHealth { get; }

        void TakeDamage(HitEventInfo hitEventInfo);
        event Action<HitEventInfo> OnHit;
        event Action OnDefeated;
    }
}