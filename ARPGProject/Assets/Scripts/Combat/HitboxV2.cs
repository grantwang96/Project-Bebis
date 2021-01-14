using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis
{
    public class HitboxV2 : MonoBehaviour
    {
        [SerializeField] private HitboxInfoV2 _info;

        public void Initialize(HitboxInfoV2 info) {
            _info = info;
        }

        public void Clear() {
            _info = null;
        }

        private void OnTriggerEnter(Collider collider) {
            _info?.HitboxTriggered(this, collider);
        }
    }

    public class HitboxInfoV2 : HitboxInfo
    {

        public readonly Action<HitboxV2, Collider> OnHitboxTriggered;

        public HitboxInfoV2(Action<HitboxV2, Collider> onHitboxTriggered) {
            OnHitboxTriggered = onHitboxTriggered;
        }

        public void HitboxTriggered(HitboxV2 hitBox, Collider collider) {
            OnHitboxTriggered?.Invoke(hitBox, collider);
        }
    }
}
