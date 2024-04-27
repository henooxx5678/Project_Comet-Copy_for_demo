using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace DoubleHeat.DependsOnUnityInputSystem.KeystrokeEventsHandling {
    
    public class StackedKeystrokeEventsHandler : KeystrokeEventsHandler {
        
        static List<StackedKeystrokeEventsHandler> _list = new List<StackedKeystrokeEventsHandler>();


        public bool IsTheCurrentActive => _list != null && _list.Count > 0 && _list[_list.Count - 1] == this;


        protected virtual void OnEnable () {
            StartCoroutine(WaitingForOneFrame( () => {
                ResetAllEventHandle();
                _list.Add(this);
            } ));
        }

        protected virtual void OnDisable () {
            _list.Remove(this);
        }


        protected override void Update () {
            base.Update();
            // if (Keyboard.current[Key.LeftCtrl].wasPressedThisFrame) {
            //     if (_list != null && _list.Count > 0 && _list[_list.Count - 1] == this) {
            //         print(gameObject.name);
            //     }
            // }
        }


        protected override void OnKeyPressed (KeystrokeEventHandle handle) {
            if (IsTheCurrentActive) {
                base.OnKeyPressed(handle);
            }
        }

        protected override void OnKeyReleased (KeystrokeEventHandle handle) {
            if (IsTheCurrentActive) {
                base.OnKeyReleased(handle);
            }
        }



        IEnumerator WaitingForOneFrame (Action callback) {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

    }
}