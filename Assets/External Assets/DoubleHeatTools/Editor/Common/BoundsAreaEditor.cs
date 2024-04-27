using UnityEngine;
using UnityEditor;

namespace DoubleHeat.Common {

    [UnityEditor.CustomEditor(typeof(BoundsArea))]
    [CanEditMultipleObjects]
    public class BoundsAreaEditor : Editor {

        BoundsArea _castedTarget;

        void OnEnable () {
            _castedTarget = (BoundsArea) target;
        }


        void OnSceneGUI() {
            if (_castedTarget) {
                Handles.DrawWireCube(_castedTarget.bounds.center, _castedTarget.bounds.size);
            }
        }

    }
}
