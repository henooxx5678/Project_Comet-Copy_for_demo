using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace DoubleHeat.Utilities {

    public static class TransformExtensions {

        public static void SetPosX (this Transform transform, float x) {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        public static void SetPosY (this Transform transform, float y) {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetPosZ (this Transform transform, float z) {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        public static void SetPosXY (this Transform transform, Vector2 v2) {
            Vector3 pos = transform.position;
            pos.x = v2.x;
            pos.y = v2.y;
            transform.position = pos;
        }

        public static void SetLocalPosX (this Transform transform, float x) {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        public static void SetLocalPosY (this Transform transform, float y) {
            Vector3 pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
        }

        public static void SetLocalPosZ (this Transform transform, float z) {
            Vector3 pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
        }

        public static void SetLocalPosXY (this Transform transform, Vector2 v2) {
            Vector3 pos = transform.localPosition;
            pos.x = v2.x;
            pos.y = v2.y;
            transform.localPosition = pos;
        }

        public static void SetLocalScaleX (this Transform transform, float x) {
            Vector3 scale = transform.localScale;
            scale.x = x;
            transform.localScale = scale;
        }

        public static void SetLocalScaleY (this Transform transform, float y) {
            Vector3 scale = transform.localScale;
            scale.y = y;
            transform.localScale = scale;
        }

        public static void SetLocalScaleZ (this Transform transform, float z) {
            Vector3 scale = transform.localScale;
            scale.z = z;
            transform.localScale = scale;
        }

        public static void SetTransformValuesNonLocal (this Transform transform, Transform targetTrans) {
            transform.position   = targetTrans.position;
            transform.rotation   = targetTrans.rotation;
            transform.localScale = targetTrans.localScale;
        }

        public static void SetTransformValuesLocal (this Transform transform, Transform targetTrans) {
            transform.localPosition = targetTrans.localPosition;
            transform.localRotation = targetTrans.localRotation;
            transform.localScale    = targetTrans.localScale;
        }

        public static void SwitchParent (this Transform selfTrans, Transform targetParent) {
            Vector3 localPos = selfTrans.localPosition;
            Quaternion localRot = selfTrans.localRotation;
            Vector3 localScale = selfTrans.localScale;

            selfTrans.parent = targetParent;

            selfTrans.localPosition = localPos;
            selfTrans.localRotation = localRot;
            selfTrans.localScale = localScale;
        }

        public static void DestroyAllChildren (this Transform transform) {
            for (int i = transform.childCount - 1 ; i >= 0 ; i--) {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyImmediateAllChildren (this Transform transform) {
            for (int i = transform.childCount - 1 ; i >= 0 ; i--) {
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void SetLayerRecursively (this Transform transform, int layer) {
            transform.gameObject.layer = layer;

            foreach (Transform child in transform) {
                if (child != null) {
                    child.SetLayerRecursively(layer);
                }
            }
        }

    }
}
