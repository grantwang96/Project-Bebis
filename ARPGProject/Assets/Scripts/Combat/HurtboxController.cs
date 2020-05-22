using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bebis {
    public class HurtboxController : MonoBehaviour {

        [SerializeField] private GameObject _damageableGO;
        [SerializeField] private List<Hurtbox> _hurtBoxObjs = new List<Hurtbox>();

        private readonly Dictionary<string, Hurtbox> _hurtBoxes = new Dictionary<string, Hurtbox>();
        public IReadOnlyDictionary<string, Hurtbox> Hurtboxes => _hurtBoxes;

        private bool _receivedHit;
        private HitEventInfo _hitEventInfo;

        public event Action<HitEventInfo> OnHit;

        private void Awake() {
            _hurtBoxes.Clear();
            IDamageable damageable = _damageableGO.GetComponent<IDamageable>();
            for(int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxes.Add(_hurtBoxObjs[i].name, _hurtBoxObjs[i]);
                _hurtBoxObjs[i].Initialize(damageable);
                _hurtBoxObjs[i].OnHit += OnHurtboxHit;
            }
        }

        private void LateUpdate() {
            if (_receivedHit) {
                ProcessHit();
            }
        }

        private void OnDestroy() {
            for(int i = 0; i < _hurtBoxObjs.Count; i++) {
                _hurtBoxObjs[i].OnHit -= OnHurtboxHit;
            }
        }

        private void OnHurtboxHit(HitEventInfo hitInfo) {
            _receivedHit = true;
            // TODO: have comparison take into account various hits on the same frame and have one take priority
            _hitEventInfo = hitInfo;
        }

        private void ProcessHit() {
            _receivedHit = false;
            OnHit?.Invoke(_hitEventInfo);
        }
    }
}
