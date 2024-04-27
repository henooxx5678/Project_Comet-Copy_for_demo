using UnityEngine;

namespace DoubleHeat.Display {

    [RequireComponent(typeof(SpriteRenderer))]
    public class HideSpriteAtRuntime : MonoBehaviour {

        public static bool isActive;


        void Start () {
            if (isActive)
                GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}
