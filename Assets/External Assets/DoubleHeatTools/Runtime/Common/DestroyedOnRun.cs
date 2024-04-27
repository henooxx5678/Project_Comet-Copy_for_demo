using UnityEngine;

namespace DoubleHeat.Common {

    public class DestroyedOnRun : MonoBehaviour {

        public bool isEnabled = true;

        void Awake () {
            if (isEnabled)
                Destroy(gameObject);
        }
    }
}
