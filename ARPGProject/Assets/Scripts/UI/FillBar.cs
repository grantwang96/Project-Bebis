using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class FillBar : MonoBehaviour {

        [SerializeField] private Image _fillBackground;
        [SerializeField] private Image _fill;

        public void SetFillPercent(float value) {
            Vector2 fillSize = _fillBackground.rectTransform.sizeDelta;
            fillSize.x = Mathf.Clamp(fillSize.x * value, 0f, fillSize.x);
            _fill.rectTransform.sizeDelta = fillSize;
        }
    }
}
