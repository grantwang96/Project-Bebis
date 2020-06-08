using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    
    public class CombatHitboxInfo2D : HitboxInfo2D
    {
        public CombatHitboxInfo2D(Action<Hitbox, Collider2D> onHitboxTriggered) : base(onHitboxTriggered) {

        }
    }

    public class CombatHitboxInfo3D : HitboxInfo3D {

        public CombatHitboxInfo3D(Action<Hitbox, Collider> onHitboxTriggered) : base(onHitboxTriggered) {

        }
    }
}
