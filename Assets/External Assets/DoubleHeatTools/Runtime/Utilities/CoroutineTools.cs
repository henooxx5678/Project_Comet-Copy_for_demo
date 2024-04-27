using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DoubleHeat.Utilities {


    public static class CoroutineTools {

        public static IEnumerator CoolingDown(float duration, Func<float> currentTimeGetter, Action<float> remainedTimeUpdater = null, Action<float> progessUpdater = null) {
            float startTime = currentTimeGetter.Invoke();
            float elapsedTime = 0f;

            while (elapsedTime < duration) {
                UpdateState();
                yield return null;
                elapsedTime = currentTimeGetter.Invoke() - startTime;
            }
            UpdateState();


            void UpdateState() {
                remainedTimeUpdater?.Invoke(duration - elapsedTime);
                if (duration > 0) {
                    progessUpdater?.Invoke(elapsedTime / duration);
                }
            }
        }
    }

}
