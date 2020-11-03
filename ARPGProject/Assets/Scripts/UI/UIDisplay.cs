using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public interface IUIDisplay
    {
        void Initialize(UIDisplayInitializationData initializationData);
        void OnHide();
        void Destroy();
    }

    public abstract class BaseUIDisplay : MonoBehaviour, IUIDisplay
    {
        public abstract void Initialize(UIDisplayInitializationData initializationData);
        public abstract void OnHide();
        public virtual void Destroy() {
            Destroy(gameObject);
        }
    }

    public class UIDisplayInitializationData
    {
        public static UIDisplayInitializationData Empty = new UIDisplayInitializationData();
    }
}
