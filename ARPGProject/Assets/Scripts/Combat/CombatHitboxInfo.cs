using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class CombatHitboxInfo : HitboxInfo
    {
        public CombatHitboxInfo(Action<Hitbox, Hurtbox> onHitboxTriggered) : base(onHitboxTriggered) {

        }
    }
}
