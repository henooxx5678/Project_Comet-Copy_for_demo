using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleHeat.Common {

    public class DelayedEventsEmitter : MonoBehaviour {

        [SerializeField] DelayedEventInfo[] _delayedEventInfos;


        void OnEnable() {
            foreach (DelayedEventInfo info in _delayedEventInfos) {
                StartCoroutine(info.DelayingToEmitEvent());
            }
        }



        [Serializable]
        class DelayedEventInfo {
            [SerializeField] float _delay;
            [SerializeField] UnityEvent _targetEvent;
            
            public float Delay => _delay;


            public IEnumerator DelayingToEmitEvent() {
                yield return new WaitForSeconds(Delay);
                _targetEvent?.Invoke();
            }
        }

    }
}