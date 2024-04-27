using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DoubleHeat.UI;

namespace ProjectComet.OptionsHandling.Display {
    
    public class RebindingStatusDisplayController : MonoBehaviour {
    
        [SerializeField] RebindingDisplayUnitInfo[] _rebindingStatusDisplayUnitInfos;

        public UnityEvent onDuplicatedRebindingWarning;

        protected virtual void OnEnable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.RebindingStatusUpdated += OnRebindingStatusUpdate;
                OptionsHolder.current.DuplicatedRebindPerformed += OnDuplicatedRebindPerformed;
            }
        }

        protected virtual void OnDisable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.RebindingStatusUpdated -= OnRebindingStatusUpdate;
                OptionsHolder.current.DuplicatedRebindPerformed -= OnDuplicatedRebindPerformed;
            }
        }


        public void UpdateStatus (string currentRebindingActionPath) {
            foreach (RebindingDisplayUnitInfo info in _rebindingStatusDisplayUnitInfos) {
                info.IsListening = info.ActionPath == currentRebindingActionPath;
            }
        }


        protected virtual void OnRebindingStatusUpdate (string currentRebindingActionPath) {
            UpdateStatus (currentRebindingActionPath);
        }

        protected virtual void OnDuplicatedRebindPerformed () {
            onDuplicatedRebindingWarning?.Invoke();
        }


        [Serializable]
        public class RebindingDisplayUnitInfo {
            [SerializeField] string _actionPath;
            [SerializeField] Animator _animator;

            public string ActionPath => _actionPath;

            bool _isListening = false;
            public bool IsListening {
                get => _isListening;
                set {
                    _isListening = value;
                    _animator.SetBool("Is Listening", _isListening);
                }
            }
        
        }

    }

}
