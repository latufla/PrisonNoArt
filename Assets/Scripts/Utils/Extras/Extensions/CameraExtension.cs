using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class CameraExtension
    {
        public static bool IsWorldPointVisible(this Camera camera, Vector3 point)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            return 0.0f <= viewportPoint.x && viewportPoint.x <= 1.0f
                && 0.0f <= viewportPoint.y && viewportPoint.y <= 1.0f
                && viewportPoint.z > 0.0f;
        }


        public static Vector3 WorldToUiSpace(this Camera worldCamera,
            Canvas canvas,
            Vector3 position)
        {
            Vector3 screenPos = worldCamera.WorldToScreenPoint(position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out Vector2 movePos);
            return canvas.transform.TransformPoint(movePos);
        }
    }
}
