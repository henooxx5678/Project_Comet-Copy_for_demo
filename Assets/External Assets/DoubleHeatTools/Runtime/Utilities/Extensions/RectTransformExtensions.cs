using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using DoubleHeat.Common;

namespace DoubleHeat.Utilities {

    public static class RectTransformExtensions {

        public static void SetAnchoredPosX (this RectTransform rectTransform, float x) {
            Vector3 pos = rectTransform.anchoredPosition;
            pos.x = x;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetAnchoredPosY (this RectTransform rectTransform, float y) {
            Vector3 pos = rectTransform.anchoredPosition;
            pos.y = y;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetRectTransformValues (this RectTransform thisRectTrans, RectTransform targetRectTrans) {
            thisRectTrans.anchorMin        = targetRectTrans.anchorMin;
            thisRectTrans.anchorMax        = targetRectTrans.anchorMax;
            thisRectTrans.anchoredPosition = targetRectTrans.anchoredPosition;
            thisRectTrans.sizeDelta        = targetRectTrans.sizeDelta;
        }

        public static void SetPivotToSpritePivot (this RectTransform rectTrans, Sprite sprite) {
            Vector2 size = sprite.rect.size;
            Vector2 pixelPivot = sprite.pivot;
            Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
            rectTrans.pivot = percentPivot;
        }


        public static bool IsInViewport (this RectTransform rectTrans, Camera cam) {

            FloatRange insideRange = new FloatRange(0f, 1f);

            Vector3[] corners = new Vector3[4];
            rectTrans.GetWorldCorners(corners);

            foreach (Vector3 corner in corners) {
                Vector3 viewportPoint = cam.WorldToScreenPoint(corner);
                if (insideRange.IsInRange(viewportPoint.x) && insideRange.IsInRange(viewportPoint.y))
                    return true;
            }
            return false;
        }

    }
}
