using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoubleHeat.Common {

    public class CoroutineSequencer {

        MonoBehaviour _coroutineCarrier = null;

        public CoroutineSequencer (MonoBehaviour coroutineCarrier) {
            _coroutineCarrier = coroutineCarrier;
        }


        IEnumerator Sequencing<T> (Func<float> currentTimeGetter, IEnumerable<T> elements, Action<T> outputSetter, float intervalBetweenSteps, SequencingOptions options = default) {
            T[] elementsArray = elements.ToArray();

            int progressCount = elementsArray.Length;
            if (options.pingPong) {
                progressCount *= 2;
                if (!options.repeatReflectionPointWhenPingPong) {
                    progressCount -= 1;
                    progressCount = Math.Max(progressCount, 0);
                }
            }

            float duration = progressCount * intervalBetweenSteps;

            float startTime = currentTimeGetter.Invoke();
            float elapsedTime = 0f;

            int prevSetElementIndex = -1;
            while (elapsedTime < duration) {

                elapsedTime = currentTimeGetter.Invoke() - startTime;
                if (options.loop) {
                    elapsedTime %= duration;
                }

                int progressIndex = (int) (elapsedTime / intervalBetweenSteps);
                progressIndex = Math.Max(progressIndex, 0);
                progressIndex = Math.Min(progressIndex, progressCount - 1);

                int elementIndex = progressIndex;
                if (elementIndex > elementsArray.Length) {
                    if (options.pingPong) {
                        elementIndex = (progressCount - 1) - elementIndex;
                    }
                    elementIndex %= elementsArray.Length;
                }

                if (elementIndex != prevSetElementIndex) {
                    outputSetter.Invoke(elementsArray[elementIndex]);
                    prevSetElementIndex = elementIndex;
                }

                yield return null;
            }

            options.endCallback?.Invoke();
        }


        public struct SequencingOptions {
            public bool pingPong;
            public bool repeatReflectionPointWhenPingPong;
            public bool loop;
            public Action endCallback;
        }

    }
}