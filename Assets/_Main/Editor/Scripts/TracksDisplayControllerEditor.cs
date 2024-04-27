using UnityEngine;
using UnityEditor;

namespace ProjectComet.CoreGameplay.Display {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TracksDisplayController))]
    public class TracksDisplayControllerEditor : Editor {


        readonly Color _spawnPositionShowingColor = Color.white;
        readonly Vector3 _spawnPositionShowingCubeSize = Vector3.one * 1.5f;


        TracksDisplayController _castedTarget;
        public TracksDisplayController CastedTarget {
            get {
                _castedTarget ??= (TracksDisplayController) target;
                return _castedTarget;
            }
        }


        void OnSceneGUI () {
            Handles.color = _spawnPositionShowingColor;

            Transform transform = CastedTarget.transform;

            Vector3 spawnPosition = transform.TransformPoint(Vector3.forward * CastedTarget.PreOnsetDistance);

            for (int i = 0 ; i < CastedTarget.TracksAmount ; i++) {
                Handles.DrawWireCube(CastedTarget.GetSpawnPositionOfTrack(i), _spawnPositionShowingCubeSize);
            }
            // Handles.DrawWireDisc(spawnPosition, transform.forward, 5);
        }

    }
}
