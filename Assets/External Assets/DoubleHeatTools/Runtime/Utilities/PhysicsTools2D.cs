using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class PhysicsTools2D {

        public static Vector2 GetFinalDeltaPosAwaringObstacle (Rigidbody2D rigidbody, Vector2 dir, float distance, LayerMask mask) {

            // Collide With Obstacles (Detect)
            List<RaycastHit2D> hits    = new List<RaycastHit2D>();
            ContactFilter2D    filter  = new ContactFilter2D();
            filter.useTriggers = false;
            filter.SetLayerMask(mask);;

            Vector2 finalDeltaPos = dir * distance;

            if (rigidbody.Cast(dir, filter, hits, distance) > 0) {

                Vector2 totalComp = Vector2.zero;

                foreach (var hit in hits) {

                    float totalCompProjectLength = Vector2.Dot(totalComp, hit.normal);
                    float compensationLength = Vector2.Dot(dir * (hit.distance - distance), hit.normal) - totalCompProjectLength;

                    if (compensationLength > 0) {
                        Vector2 compensation = compensationLength * hit.normal;
                        totalComp += compensation;
                    }
                }

                finalDeltaPos += totalComp;
            }

            return finalDeltaPos;
        }

    }

}