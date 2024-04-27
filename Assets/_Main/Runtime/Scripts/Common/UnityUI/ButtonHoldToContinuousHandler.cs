using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityUI {

    [RequireComponent(typeof(Button))]
    public class ButtonHoldToContinuousHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        
        Button _button;
        public Button TargetButton => _button ??= this.GetComponent<Button>();
        public bool IsButtonInteractable => TargetButton && TargetButton.interactable;


        const float START_CONTINUOUS_DELAY = 0.5f;
        [SerializeField] float _overrideStartContinuousDelay = -1f;
        protected float startContinuousDelay => _overrideContinuousInterval < 0 ? START_CONTINUOUS_DELAY : _overrideContinuousInterval;

        const float CONTINUOUS_INTERVAL = 0.2f;
        [SerializeField] float _overrideContinuousInterval = -1f;
        protected float continuousInterval => _overrideContinuousInterval < 0 ? CONTINUOUS_INTERVAL : _overrideContinuousInterval;


        Coroutine _currentWaitingForStart = null;
        protected Coroutine currentWaitingForStart {
            get => _currentWaitingForStart;
            set {
                if (_currentWaitingForStart != null) {
                    StopCoroutine(_currentWaitingForStart);
                }
                _currentWaitingForStart = value;
            }
        }
        Coroutine _currentBeingContinuous = null;
        protected Coroutine currentBeingContinuous {
            get => _currentBeingContinuous;
            set {
                if (_currentBeingContinuous != null) {
                    StopCoroutine(_currentBeingContinuous);
                }
                _currentBeingContinuous = value;
            }
        }



        public void OnPointerDown (PointerEventData data) {
            if (IsButtonInteractable) {
                currentWaitingForStart = StartCoroutine(WaitingForStartContinuous());
            }
        }

        public void OnPointerUp (PointerEventData data) {
            currentWaitingForStart = null;
            currentBeingContinuous = null;
        }


        IEnumerator WaitingForStartContinuous () {
            yield return new WaitForSecondsRealtime(startContinuousDelay);
            currentBeingContinuous = StartCoroutine(BeingContinuous());
        }

        IEnumerator BeingContinuous () {
            while (true) {
                if (IsButtonInteractable) {
                    TargetButton.onClick?.Invoke();
                }
                yield return new WaitForSecondsRealtime(continuousInterval);
            }
        }

    }
}
