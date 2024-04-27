using Math = System.Math;

using UnityEngine;

using DoubleHeat.Utilities;

namespace DoubleHeat.Display.Graphics {

    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererArrowHandler : MonoBehaviour {

        [Header("Properties")]
        public float lineWdith   = 0.1f;
        public float headLength  = 0.3f;
        public float headWidth   = 0.2f;
        public float neckGapRate = 0.001f;

        [Header("Start & End")]
        [SerializeField] Vector3 _startPos;
        [SerializeField] Vector3 _endPos;

        LineRenderer _lineRenderer;
        public LineRenderer TargetLineRenderer {
            get {
                if (_lineRenderer == null)
                    _lineRenderer = GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }

        void Update () {
            UpdateShape();
        }

        public void SetStartEnd (Vector3 start, Vector3 end) {
            _startPos = start;
            _endPos   = end;
            UpdateShape();
        }

        void UpdateShape () {
            float length = (_endPos - _startPos).magnitude;
            float neckPosRate = (length - headLength) / length;

            TargetLineRenderer.positionCount = 4;

            TargetLineRenderer.SetPositions( new Vector3[] {
                _startPos,
                Vector3.Lerp(_startPos, _endPos, Mathf.Max(neckPosRate - neckGapRate, 0f)),
                Vector3.Lerp(_startPos, _endPos, Mathf.Max(neckPosRate, 0f)),
                _endPos
            } );

            TargetLineRenderer.widthCurve = new AnimationCurve(
                new Keyframe(0, lineWdith),
                new Keyframe(Mathf.Max(neckPosRate - neckGapRate, 0f), lineWdith),
                new Keyframe(Mathf.Max(neckPosRate, 0f), headWidth),
                new Keyframe(1f, 0f)
            );
        }

    }
}
