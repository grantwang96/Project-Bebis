using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface UIObject {
        bool Initialize(UIObjectInitData initData); // setting up the ui object
        void Dispose(); // good for unhooking to events
    }

    public class UIObjectInitData {
        public static UIObjectInitData Empty = new UIObjectInitData();
    }
}
