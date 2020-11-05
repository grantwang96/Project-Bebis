using UnityEngine;
using Winston;

namespace Bebis {
    public interface ICharacter : IPooledObject {

        string UniqueId { get; }

        IDamageable Damageable { get; }
        IMoveController MoveController { get; }
        IActionController ActionController { get; }
        IAnimationController AnimationController { get; }
        ICharacterStatManager CharacterStatManager { get; }
        ITargetManager TargetManager { get; }
        IMoveControllerInfoProvider MoveInfoProvider { get; }
        IActionControllerInfoProvider ActionInfoProvider { get; }

        HitboxController HitboxController { get; }
        HurtboxController HurtboxController { get; }
    }

    public class CharacterInitializationData : PooledObjectInitializationData
    {
        public string UniqueId;
        public Vector3 SpawnLocation;
    }
}