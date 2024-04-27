using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class RigidbodyExtensions {

        public static Collider[] GetColliders (this Rigidbody rigidbody) {
            return rigidbody.gameObject.GetComponentsInChildren<Collider>();
        }

        public static void ActionToColliders (this Rigidbody rigidbody, Action<Collider> action) {
            foreach (Collider collider in rigidbody.GetColliders()) {
                action?.Invoke(collider);
            }
        }
    }



}
