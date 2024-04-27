using UnityEngine;

namespace DoubleHeat.Utilities {

    public static class MonoBehaviourTools {

        public static T GetConvertedMonoBehaviour<T> (MonoBehaviour target) where T: IMonoBehaviourAttached {
            if (target && target is IMonoBehaviourAttached) {
                return (T) (IMonoBehaviourAttached) target;
            }
            return default;
        }

    }
}