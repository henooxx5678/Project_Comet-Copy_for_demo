using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace DoubleHeat.DependsOnUnityInputSystem.KeystrokeEventsHandling {
    
    public class SecretCodeHandler : MonoBehaviour {

        [SerializeField] Key[] _keysOfSecretCode;

        public UnityEvent onTrigger;

        int _currentWaitingKeyIndex = 0;

        protected virtual void Update () {
            if (_currentWaitingKeyIndex < _keysOfSecretCode.Length) {
                if (Keyboard.current[_keysOfSecretCode[_currentWaitingKeyIndex]].wasPressedThisFrame) {
                    _currentWaitingKeyIndex++;
                }
            }

            if (_keysOfSecretCode.Length > 0 && _currentWaitingKeyIndex >= _keysOfSecretCode.Length) {
                onTrigger?.Invoke();
                _currentWaitingKeyIndex = 0;
            }
        }

    }

}