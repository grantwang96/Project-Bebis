using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis
{
    public class MonoBehaviourMetaData : MonoBehaviour
    {
        public static MonoBehaviourMetaData Instance { get; private set; }

        public LayerMask NPCVisionLayerMask => _npcVisionLayerMask;

        [SerializeField] private LayerMask _npcVisionLayerMask;

        private void Awake() {
            Instance = this;
        }
    }
}
