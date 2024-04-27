using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityUI {

    public class PointerPressedScalingHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [SerializeField] bool _interactable;
        public bool Interactable => _interactable;

        protected float scaling {
            get {
                if (GlobalButtonScalingReactSettingsHandler.current) {
                    return GlobalButtonScalingReactSettingsHandler.current.Scaling;
                }
                return 1f;
            }
        }

        protected float minDuration {
            get {
                if (GlobalButtonScalingReactSettingsHandler.current) {
                    return GlobalButtonScalingReactSettingsHandler.current.MinDuration;
                }
                return 0f;
            }
        }


        Vector3 _initLocalScale = Vector3.one;

        bool _isPressed = false;
        bool _isReleaseCoolingDown = false;

        Coroutine _currentReleaseCoolingDown = null;
        protected Coroutine currentReleaseCoolingDown {
            get => _currentReleaseCoolingDown;
            set {
                if (_currentReleaseCoolingDown != value) {
                    if (_currentReleaseCoolingDown != null) {
                        StopCoroutine(_currentReleaseCoolingDown);
                    }
                    _currentReleaseCoolingDown = value;
                }
            }
        }


        protected virtual void Awake() {
            _initLocalScale = this.transform.localScale;
        }

        public void OnPointerDown (PointerEventData data) {
            if (Interactable) {
                _isPressed = true;
                PerformReact();

                currentReleaseCoolingDown = StartCoroutine(ReleaseCoolingdown());
            }
        }

        public void OnPointerUp (PointerEventData data) {
            _isPressed = false;
            
            if (!_isReleaseCoolingDown) {
                Reset();
            }
        }

        IEnumerator ReleaseCoolingdown () {
            _isReleaseCoolingDown = true;
            yield return new WaitForSecondsRealtime(minDuration);
            _isReleaseCoolingDown = false;

            if (!_isPressed) {
                Reset();
            }
        }


        protected void PerformReact () {
            this.transform.localScale = _initLocalScale * scaling;
        }

        protected void Reset () {
            this.transform.localScale = _initLocalScale;
        }

    }

}