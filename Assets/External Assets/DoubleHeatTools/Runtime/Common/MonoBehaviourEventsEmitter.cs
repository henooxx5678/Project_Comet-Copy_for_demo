using UnityEngine;
using UnityEngine.Events;

namespace DoubleHeat.Common {
    
    public class MonoBehaviourEventsEmitter : MonoBehaviour {

        public UnityEvent onAwake;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        public UnityEvent onStart;
        public UnityEvent onDestroy;


        protected virtual void Awake () {
            onAwake?.Invoke();
        }

        protected virtual void OnEnable () {
            onEnable?.Invoke();
        }

        protected virtual void OnDisable () {
            onDisable?.Invoke();
        }

        protected virtual void Start () {
            onStart?.Invoke();
        }

        protected virtual void OnDestroy () {
            onDestroy?.Invoke();
        }

    }

}