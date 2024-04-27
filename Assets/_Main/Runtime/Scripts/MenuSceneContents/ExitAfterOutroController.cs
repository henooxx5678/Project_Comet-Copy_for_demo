using System;
using UnityEngine;
using UnityEngine.Events;
using DoubleHeat.Utilities;
using ProjectComet.Common.UnityAnimations;
using ProjectComet.Levels;
using ProjectComet.UIDisplay;
using ProjectComet.MenuSceneContents.Display;

namespace ProjectComet.MenuSceneContents {
    
    public class ExitAfterOutroController : MonoBehaviour {

        [SerializeField] OutroEndAnimationEventTransmitter _outroEndAnimationEventTransmitter;

        [SerializeField] ExitingHandle[] _exitingHandles;


        public UnityEvent StartOutro;


        ExitingHandle _currentHandle = null;


        protected virtual void OnEnable () {
            _currentHandle = null;

            if (_outroEndAnimationEventTransmitter) {
                _outroEndAnimationEventTransmitter.OutroEnded += OnOutroEnd;
            }
        }
        
        protected virtual void OnDisable () {
            if (_outroEndAnimationEventTransmitter) {
                _outroEndAnimationEventTransmitter.OutroEnded -= OnOutroEnd;
            }
        }


        public void StartExiting (string name) {
            if (_currentHandle == null) {
                _currentHandle = Array.Find(_exitingHandles, handle => handle.name == name);
                StartOutro?.Invoke();
            }
        }


        protected void OnOutroEnd () {
            if (_currentHandle != null) {
                _currentHandle.InvokeOutroEnd();
                _currentHandle = null;
            }
            else {
                Debug.LogWarning("No exiting destination!");
            }
        }
        


        [Serializable]
        public class ExitingHandle {
            public string name;
            public UnityEvent onOutroEnd;

            public void InvokeOutroEnd () {
                Debug.Log($"Exit to \"{name}\"");
                onOutroEnd?.Invoke();
            }

        }

    }
}