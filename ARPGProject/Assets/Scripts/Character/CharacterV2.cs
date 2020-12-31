using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    /// <summary>
    /// Interface for all characters in the game
    /// </summary>
    public interface ICharacterV2
    {
        // unit controller reference
        IUnitController UnitController { get; }
        // damageable
        IDamageableV2 Damageable { get; }
        // move controller
        IMoveControllerV2 MoveController { get; }
        // action controller
        IActionControllerV2 ActionController { get; }
        // animation controller
        IAnimationController AnimationController { get; }
        // hitbox controller
        HitboxControllerV2 HitboxController { get; }
        // hurtbox controller
        HurtboxControllerV2 HurtboxController { get; }
        // player character
        GameObject GameObject { get; }
    }

    public interface ICharacterComponent
    {
        void Initialize(ICharacterV2 character);
    }
}
