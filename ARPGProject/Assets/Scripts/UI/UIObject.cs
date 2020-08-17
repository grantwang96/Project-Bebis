using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebis {
    public interface UIObject {
        bool Initialize(UIInitData initData); // setting up the ui object
        void Dispose(); // good for unhooking to events
    }

    public class UIInitData {
        public static UIInitData Empty = new UIInitData();
    }
}
