using UnityEngine;

namespace DoubleHeat.Common {

    public class BoundsArea : MonoBehaviour {

        [SerializeField] Vector3 _boundsSize;

        public Bounds bounds => new Bounds(transform.position, _boundsSize);

    }

}