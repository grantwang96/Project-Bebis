using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    
    public class CombatHitboxInfo3D : HitboxInfo3D {

        public CombatHitboxInfo3D(Action<Hitbox, Collider> onHitboxTriggered) : base(onHitboxTriggered) {

        }
    }
}
