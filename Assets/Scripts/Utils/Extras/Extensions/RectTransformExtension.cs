using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class RectTransformExtension
    {
        public static RectTransform SetAnchorMin(RectTransform t, Vector2 anchorMin)
        {
            t.anchorMin = anchorMin;
            return t;
        }


        public static RectTransform SetAnchorMax(RectTransform t, Vector2 anchorMax)
        {
            t.anchorMax = anchorMax;
            return t;
        }
    }
}
