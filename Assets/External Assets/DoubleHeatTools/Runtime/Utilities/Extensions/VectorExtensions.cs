using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace DoubleHeat.Utilities {

    public static class VectorExtensions {

        // == Vector2 ==
        public static Vector2 GetAfterSetX (this Vector2 v, float x) {
            v.x = x;
            return v;
        }

        public static Vector2 GetAfterSetY (this Vector2 v, float y) {
            v.y = y;
            return v;
        }

        public static Vector2 DirectionTo (this Vector2 v0, Vector2 v1) {
            return (v1 - v0).normalized;
        }

        public static Vector2 GetRotateTowards (this Vector2 currentDir, Vector2 destinationDir, float maxAngleDelta) {
            float destinationRotAngle = Vector2.SignedAngle(currentDir, destinationDir);
            float destinationRotDir   = Mathf.Sign(destinationRotAngle);
            float actualRotAngle = destinationRotDir * Mathf.Min( Mathf.Abs(destinationRotAngle), maxAngleDelta );

            return Quaternion.AngleAxis(actualRotAngle, Vector3.forward) * currentDir;
        }


        // == Vector3 ==
        public static Vector3 GetAfterSetX (this Vector3 v, float x) {
            v.x = x;
            return v;
        }


        public static Vector3 GetAfterSetY (this Vector3 v, float y) {
            v.y = y;
            return v;
        }

        public static Vector3 GetAfterSetZ (this Vector3 v, float z) {
            v.z = z;
            return v;
        }

        public static Vector3 DirectionTo (this Vector3 v0, Vector3 v1) {
            return (v1 - v0).normalized;
        }



        // == Color ==
        public static Color GetAfterSetA (this Color c, float a) {
            c.a = a;
            return c;
        }

    }
}
