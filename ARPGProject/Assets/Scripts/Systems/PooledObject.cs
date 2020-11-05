using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public interface IPooledObject
    {
        void SetActive(bool active);
        void Initialize(PooledObjectInitializationData initializationData);
        IPooledObject Spawn();
        void Despawn();
    }

    public class PooledObjectInitializationData
    {

    }
}
