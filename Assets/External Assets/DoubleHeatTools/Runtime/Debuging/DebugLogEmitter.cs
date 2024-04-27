using UnityEngine;

namespace DoubleHeat.Debugging {
    public class DebugLogEmitter : MonoBehaviour {

        public void EmitLog (string logString) {
            Debug.Log(logString);
        }
        public void EmitWarning (string logString) {
            Debug.LogWarning(logString);
        }
        public void EmitError (string logString) {
            Debug.LogError(logString);
        }

    }

}