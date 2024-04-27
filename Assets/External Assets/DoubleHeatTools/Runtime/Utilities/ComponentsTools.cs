using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DoubleHeat.Utilities {

    public static class ComponentsTools {

        public static void SetAndKeepAttachedGameObjectUniquely<T>(ref T container, T newSetted) where T : MonoBehaviour {
            if (container != null && container.gameObject != null)
                Object.Destroy(container.gameObject);

            container = newSetted;
        }
    }

}