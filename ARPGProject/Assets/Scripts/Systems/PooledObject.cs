using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public interface IPooledObject
    {
        void SetActive(bool active);
        IPooledObject Spawn();
        void Despawn();
    }
}
