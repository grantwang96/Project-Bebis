using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class CombatHitboxInfo : HitboxInfo
    {
        public CombatHitboxInfo(Action<Hitbox, Collider2D> onHitboxTriggered) : base(onHitboxTriggered) {

        }
    }
}
