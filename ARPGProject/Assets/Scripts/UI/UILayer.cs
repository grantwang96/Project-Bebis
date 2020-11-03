using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Winston
{
    public class UILayer : MonoBehaviour
    {
        [SerializeField] private UILayerId _uiLayerId;
        public UILayerId UILayerId => _uiLayerId;
    }

    public enum UILayerId
    {
        None,
        Background,
        Main,
        Notifications
    }
}
