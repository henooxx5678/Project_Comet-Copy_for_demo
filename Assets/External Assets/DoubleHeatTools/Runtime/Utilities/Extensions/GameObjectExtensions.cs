using System.Collections.Generic;
using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class GameObjectExtensions {


        public static void SetLayerOfChildren (this GameObject gameObject, int layer, bool includeSelf = false) {
            if (includeSelf) {
                gameObject.layer = layer;
            }

            foreach (Transform child in gameObject.transform) {
                if (child != null) {
                    child.gameObject.SetLayerOfChildren(layer, true);
                }
            }
        }

    }



}
