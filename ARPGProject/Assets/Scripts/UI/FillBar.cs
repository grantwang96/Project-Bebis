using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bebis {
    public class FillBar : MonoBehaviour {

        [SerializeField] private Image _fillBackground;
        [SerializeField] private Image _fill;
        [SerializeField] private bool _useY;

        public void SetFillPercent(float value) {
            float width = _fillBackground.rectTransform.rect.width;
            float height = _fillBackground.rectTransform.rect.height;
            if (_useY) {
                height = Mathf.Clamp(height * value, 0f, height);
            } else {
                width = Mathf.Clamp(width * value, 0f, width);
            }
            _fill.rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
