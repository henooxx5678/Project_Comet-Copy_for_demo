using System.Collections.Generic;
using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class CanvasExtensions {

        public static void SetSortingLayer (this Canvas canvas, string layerName) {
            canvas.sortingLayerName = layerName;
            canvas.sortingLayerID = SortingLayer.NameToID(layerName);
        }

    }



}
