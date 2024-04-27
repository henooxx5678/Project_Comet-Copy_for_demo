using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DoubleHeat.DependsOnUnityInputSystem.KeystrokeEventsHandling {

    public class KeystrokeEventsHandler: MonoBehaviour {


        [SerializeField] InputHandlerType _inputHandlerType;

        [Header("Press Event")]
        [SerializeField] KeystrokeEventHandle[] _keystrokeEventHandles;

        [Header("Hold Event")]
        [SerializeField] float _holdingMinDuration = 1f;
        [SerializeField] HoldedKeystrokeEventHandle[] _holdedKeyStrokeEventHandles;
        
    
        protected IEnumerable<KeystrokeEventHandle> allKeystrokeEventHandles {
            get {
                List<KeystrokeEventHandle> result = new List<KeystrokeEventHandle>();

                if (_keystrokeEventHandles != null) {
                    result.AddRange(_keystrokeEventHandles);
                }
                if (_holdedKeyStrokeEventHandles != null) {
                    result.AddRange(_holdedKeyStrokeEventHandles);
                }
                
                return result;
            }
        }



        protected virtual void Update () {
            if (_inputHandlerType == InputHandlerType.InputSystemPackage_New) {

                if (allKeystrokeEventHandles != null) {
                    foreach (KeystrokeEventHandle handle in allKeystrokeEventHandles) {

                        if (Keyboard.current[handle.Key].wasPressedThisFrame) {
                            OnKeyPressed(handle);
                        }

                        if (Keyboard.current[handle.Key].wasReleasedThisFrame) {
                            OnKeyReleased(handle);
                        }
                    }
                }

                if (_holdedKeyStrokeEventHandles != null) {
                    foreach (HoldedKeystrokeEventHandle handle in _holdedKeyStrokeEventHandles) {
                        handle.CheckHoldedTime(_holdingMinDuration);
                    }
                }
            }
        }


        public void ResetAllEventHandle () {
            foreach (var handle in allKeystrokeEventHandles) {
                handle.Reset();
            }
        }

        public bool ContainsKeystrokeEventOfKey (Key key) {
            return Array.Exists(_keystrokeEventHandles, handle => handle.Key == key);
        }


        protected virtual void OnKeyPressed (KeystrokeEventHandle handle) {
            handle.OnPress();
        }

        protected virtual void OnKeyReleased (KeystrokeEventHandle handle) {
            handle.OnRelease();
        }


        [Serializable]
        protected class KeystrokeEventHandle {
            [SerializeField] Key _key;
            [SerializeField] UnityEvent _unityEvent;

            public Key Key => _key;

            public virtual void Reset () {}

            public virtual void OnPress () {
                InvokeEvent();
            }

            public virtual void OnRelease () {}


            protected virtual void InvokeEvent () {
                _unityEvent?.Invoke();
            }
        }

        [Serializable]
        protected class HoldedKeystrokeEventHandle : KeystrokeEventHandle {
            public bool IsHolded {get; protected set;} = false;
            public float PressedTime {get; protected set;} = Mathf.Infinity;
            public bool HasInvoked {get; protected set;} = false;


            [SerializeField] UnityEvent<float> onUpdateHoldedTime;
            [SerializeField] UnityEvent<float> onUpdateProgressRate;


            public override void Reset () {
                IsHolded = false;
                PressedTime = Mathf.Infinity;
                HasInvoked = false;

                onUpdateHoldedTime?.Invoke(0f);
                onUpdateProgressRate?.Invoke(0f);
            }

            public override void OnPress () {
                IsHolded = true;
                PressedTime = Time.realtimeSinceStartup;
            }

            public override void OnRelease () {
                Reset();
            }

            public void CheckHoldedTime (float targetDuration) {
                if (IsHolded) {

                    float holdedTime = Time.realtimeSinceStartup - PressedTime;

                    if (holdedTime >= targetDuration) {
                        if (!HasInvoked) {
                            InvokeEvent();

                            onUpdateHoldedTime?.Invoke(targetDuration);
                            onUpdateProgressRate?.Invoke(1f);
                        }
                    }
                    else {
                        onUpdateHoldedTime?.Invoke(holdedTime);
                        onUpdateProgressRate?.Invoke(holdedTime / targetDuration);
                    }
                }
            }


            protected override void InvokeEvent() {
                base.InvokeEvent();
                HasInvoked = true;
            }
        }

        public enum InputHandlerType {
            None,
            // InputManager_Old,
            InputSystemPackage_New
        }
    }

}