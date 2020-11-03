using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public class PooledObjectRoot : MonoBehaviour
    {
        public static PooledObjectRoot Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }
    }
}
