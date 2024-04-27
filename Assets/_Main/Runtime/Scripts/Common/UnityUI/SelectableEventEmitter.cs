using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace ProjectComet.Common.UnityUI {
    
    public class SelectableEventEmitter : MonoBehaviour, ISelectHandler, IDeselectHandler {
        
        public UnityEvent onSelect;
        public UnityEvent onDeselect;


        bool _isActive = false;

        void OnEnable() {
            _isActive = true;
        }

        void OnDisable () {
            _isActive = false;
        }



        public void OnSelect (BaseEventData data) {
            if (_isActive)
                onSelect?.Invoke();
        }

        public void OnDeselect (BaseEventData data) {
            if (_isActive)
                onDeselect?.Invoke();
        }

    }

}