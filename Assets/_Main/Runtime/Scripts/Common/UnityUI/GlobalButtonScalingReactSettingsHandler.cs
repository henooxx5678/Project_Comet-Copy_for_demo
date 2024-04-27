using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityUI {

    public class GlobalButtonScalingReactSettingsHandler : MonoBehaviour {
        
        static GlobalButtonScalingReactSettingsHandler _current = null;
        public static GlobalButtonScalingReactSettingsHandler current {
            get {
                return _current ??= FindObjectOfType<GlobalButtonScalingReactSettingsHandler>();
            }
        }


        [SerializeField] float _scaling = 0.95f;
        public float Scaling => _scaling;

        [SerializeField] float _minDuration = 0.06f;
        public float MinDuration => _minDuration;


        protected virtual void Awake () {
            _current = this;
        }

    }
}