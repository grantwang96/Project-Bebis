using UnityEngine.UI;
using UnityEngine;

namespace Bebis {
    public static class UIUtility {
        
        public static Vector2 WorldToScreenSpace(Canvas canvas, Camera camera, Vector3 position) {
            Vector2 uiPosition = new Vector2();
            Vector2 screenPosition = camera.WorldToViewportPoint(position);
            uiPosition.x = screenPosition.x * canvas.pixelRect.width;
            uiPosition.y = screenPosition.y * canvas.pixelRect.height;
            uiPosition /= canvas.scaleFactor;
            return uiPosition;
        }
    }
}
