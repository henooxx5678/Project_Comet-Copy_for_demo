using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DoubleHeat.Utilities {

    public static class TilemapExtensions {


        public static void SetOpacity (this Tilemap tilemap, float alpha) {
            Color color = tilemap.color;
            color.a = alpha;
            tilemap.color = color;
        }
    }



}
