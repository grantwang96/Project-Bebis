using System;

namespace Bebis {
    public interface IDamageable {
        int Health { get; }
        int MaxHealth { get; }
        bool Dead { get; }

        void TakeDamage(HitEventInfo hitEventInfo);
        event Action<int> OnCurrentHealthChanged;
        event Action<int> OnMaxHealthChanged;
        event Action<HitEventInfo> OnHit;
        event Action<HitEventInfo> OnHitStun;
        event Action OnDefeated;
    }
}