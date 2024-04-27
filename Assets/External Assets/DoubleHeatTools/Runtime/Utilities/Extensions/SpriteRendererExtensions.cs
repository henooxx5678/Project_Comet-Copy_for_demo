using System.Collections.Generic;
using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class SpriteRendererExtensions {


        public static void SetSortingLayer (this SpriteRenderer sr, string layerName) {
            sr.sortingLayerName = layerName;
            sr.sortingLayerID = SortingLayer.NameToID(layerName);
        }

        public static void SetOpacity (this SpriteRenderer sr, float alpha) {
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
        }

    }


}
