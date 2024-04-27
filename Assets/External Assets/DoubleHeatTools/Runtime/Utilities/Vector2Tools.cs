using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class Vector2Tools {

        public static float Direction2DToAngle (Vector2 v) {
            return Vector2.SignedAngle(Vector2.right, v);
        }

        public static Vector2 AngleToDirection2D (float angle) {
            return Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.right;
        }

        // public static Vector2 MidDirection2d


        public static Vector2 GetClosestPointInSegment (this Vector2 source, Vector2[] segment) {
            Vector2 segmentDir = (segment[1] - segment[0]).normalized;
            float segmentDistance = Vector2.Dot(segment[1] - segment[0], segmentDir);

            return segment[0] + Mathf.Clamp(Vector2.Dot(source - segment[0], segmentDir), 0f, segmentDistance) * segmentDir;
        }

    }
}
