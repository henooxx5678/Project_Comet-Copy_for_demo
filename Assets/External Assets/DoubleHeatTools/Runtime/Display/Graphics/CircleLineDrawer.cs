using Math = System.Math;

using UnityEngine;

using DoubleHeat.Utilities;

namespace DoubleHeat.Display.Graphics {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CircleLineDrawer : MonoBehaviour {

        [SerializeField] GizmosSettings gizmosSettings;
        [SerializeField] bool  _isWorldPosition = true;
        public           float radius = 5f;
        [Range(3, 600)]
        [SerializeField] int   _edgesCount = 50;
        [Range(0f, 10f)]
        [SerializeField] float _lineWidth = 1f;
        public DashedLineSettings dashedLineSettings;

        MeshFilter _meshFilter;

        void OnDrawGizmos () {
            if (gizmosSettings.drawGizmos) {
                Gizmos.color = gizmosSettings.color;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        void Awake () {
            _meshFilter = GetComponent<MeshFilter>();
        }

        void Update () {
            ReformMesh();

            if (dashedLineSettings.IsRotateEnabled) {
                transform.rotation = Quaternion.AngleAxis(dashedLineSettings.rotateSettings.GetDeltaAngle(Time.deltaTime), Vector3.forward) * transform.rotation;
            }
        }

        void ReformMesh () {

            transform.SetPosZ(0f);

            Vector3[] vertices;
            int[] triangles;

            Vector3 center = _isWorldPosition ? transform.position : Vector3.zero;
            float halfWidth = _lineWidth / 2f;

            if (dashedLineSettings.enableDashedLine) {

                int dashUnitsCount = dashedLineSettings.GetUnitsCount(radius);
                int edgesCountPerDashUnit = DashedLineSettings.GetEdgesCountPerUnit(_edgesCount, dashUnitsCount);
                int verticesCountOfLine = edgesCountPerDashUnit + 1;
                int totalVerticesCountOfLines = verticesCountOfLine * dashUnitsCount;

                vertices = new Vector3[totalVerticesCountOfLines * 2];
                triangles = new int[dashUnitsCount * edgesCountPerDashUnit * 2 * 3];


                float anglePerUnit = 360f / dashUnitsCount;
                float anglePerDash = anglePerUnit * dashedLineSettings.solidPartRate;

                float startAngle, angle;
                Vector3[] innerAndOutVertices;
                int vertStartIndex, triStartIndex;

                for (int dashI = 0 ; dashI < dashUnitsCount ; dashI++) {

                    startAngle = (dashI - dashedLineSettings.OffsetRateOfUnit) / dashUnitsCount * 360f;

                    for (int lineVertexI = 0 ; lineVertexI < verticesCountOfLine ; lineVertexI++) {

                        angle = startAngle + (float) lineVertexI / edgesCountPerDashUnit * anglePerDash;
                        innerAndOutVertices = GetInnerAndOuterVertices(center, angle, radius, halfWidth);

                        vertStartIndex = verticesCountOfLine * dashI;
                        vertices[vertStartIndex + lineVertexI]                             = _isWorldPosition ? transform.TransformDirection(transform.InverseTransformPoint(innerAndOutVertices[0])) : innerAndOutVertices[0];
                        vertices[vertStartIndex + totalVerticesCountOfLines + lineVertexI] = _isWorldPosition ? transform.TransformDirection(transform.InverseTransformPoint(innerAndOutVertices[1])) : innerAndOutVertices[1];


                        if (lineVertexI < edgesCountPerDashUnit) {
                            // inner triangle
                            triStartIndex = (dashI * edgesCountPerDashUnit + lineVertexI) * 3;
                            triangles[triStartIndex]     = vertStartIndex + lineVertexI;
                            triangles[triStartIndex + 1] = vertStartIndex + lineVertexI + 1;
                            triangles[triStartIndex + 2] = totalVerticesCountOfLines + vertStartIndex + lineVertexI;

                            // outer triangle
                            triStartIndex += dashUnitsCount * edgesCountPerDashUnit * 3;
                            triangles[triStartIndex]     = totalVerticesCountOfLines + vertStartIndex + lineVertexI;
                            triangles[triStartIndex + 1] = vertStartIndex + lineVertexI + 1;
                            triangles[triStartIndex + 2] = totalVerticesCountOfLines + vertStartIndex + lineVertexI + 1;
                        }
                    }
                }
            }
            else {

                vertices = new Vector3[_edgesCount * 2];
                triangles = new int[_edgesCount * 2 * 3];

                float angle;
                Vector3[] innerAndOutVertices;
                int triStartIndex;

                for (int i = 0 ; i < _edgesCount ; i++) {

                    angle = ((float) i / _edgesCount) * 360f;

                    innerAndOutVertices = GetInnerAndOuterVertices(center, angle, radius, halfWidth);

                    vertices[i]               = _isWorldPosition ? transform.TransformDirection(transform.InverseTransformPoint(innerAndOutVertices[0])) : innerAndOutVertices[0];
                    vertices[_edgesCount + i] = _isWorldPosition ? transform.TransformDirection(transform.InverseTransformPoint(innerAndOutVertices[1])) : innerAndOutVertices[1];

                    // inner triangle
                    triStartIndex = i * 3;
                    triangles[triStartIndex]     = i;
                    triangles[triStartIndex + 1] = (i + 1) % _edgesCount;
                    triangles[triStartIndex + 2] = _edgesCount + i;

                    // outer triangle
                    triStartIndex = (_edgesCount + i) * 3;
                    triangles[triStartIndex]     = _edgesCount + i;
                    triangles[triStartIndex + 1] = (i + 1) % _edgesCount;
                    triangles[triStartIndex + 2] = _edgesCount + (i + 1) % _edgesCount;
                }
            }

            if (_meshFilter.mesh == null) {
                _meshFilter.mesh = new Mesh();
            }
            else {
                _meshFilter.mesh.Clear();
            }

            _meshFilter.mesh.vertices = vertices;
            _meshFilter.mesh.triangles = triangles;
        }

        Vector3[] GetInnerAndOuterVertices (Vector3 center, float angle, float radius, float halfWidth) {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
            return new Vector3[2] {
                center + dir * (radius - halfWidth),
                center + dir * (radius + halfWidth)
            };
        }


        [System.Serializable]
        public class DashedLineSettings {
            const int MAX_UNITS_COUNT = 1000;

            public bool enableDashedLine = false;
            public RotateSettings rotateSettings;

            public float unitLength = 1f;
            [Range(0f, 1f)]
            public float solidPartRate = 0.5f;
            [Range(-1f, 1f)]
            public float offsetRateOfDash = 0f;

            public bool IsRotateEnabled => enableDashedLine && rotateSettings.rotateDirection != 0;
            public float OffsetRateOfUnit => offsetRateOfDash >= 0 ? solidPartRate * offsetRateOfDash : (1 - solidPartRate) * offsetRateOfDash;

            public int GetUnitsCount (float radius) {
                return Math.Min(Math.Max((int) (2 * Mathf.PI * radius / unitLength), 1), MAX_UNITS_COUNT);
            }

            public static int GetEdgesCountPerUnit (int totalEdgesCount, int unitsCount) {
                return Math.Max(totalEdgesCount / unitsCount, 1);
            }
        }

        [System.Serializable]
        public class RotateSettings {
            [Range(-1, 1)]
            public int rotateDirection;
            public float angularSpeed = 10f;

            public float GetDeltaAngle (float deltaTime) {
                return rotateDirection * angularSpeed * deltaTime;
            }
        }

        [System.Serializable]
        public class GizmosSettings {
            public bool drawGizmos = false;
            public Color color = Color.white;
        }

    }



}
