using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ProjectComet.CoreGameplay;
using ProjectComet.OptionsHandling;

namespace ProjectComet.Calibration {

    public class CalibrationResultProvider : MonoBehaviour {
        
        [SerializeField] int _times = 4;

        [Header("REFS")]
        [SerializeField] Sheet _sheet;

        public UnityEvent<float?> onNewResultGenerated;
        public UnityEvent onResultApplied;


        List<float?> _reversedDeltaTimeList = new List<float?>();


        protected virtual void OnEnable () {
            if (_sheet) {
                _sheet.NoteStruck += OnNoteStruck;
                _sheet.NoteMissed += OnNoteMissed;
            }
        }
        
        protected virtual void OnDisable () {
            _reversedDeltaTimeList.Clear();

            if (_sheet) {
                _sheet.NoteStruck -= OnNoteStruck;
                _sheet.NoteMissed -= OnNoteMissed;
            }
        }


        public float? GetAverageDeltaTimeResult () {
            float sum = 0f;
            int count = 0;
            foreach (float? reversedDelta in _reversedDeltaTimeList) {
                if (reversedDelta != null) {
                    sum += (float) reversedDelta;
                    count++;
                }
            }

            if (count > 0) {
                return sum / count;
            }
            return null;
        }

        
        public void ApplyResult () {
            if (_reversedDeltaTimeList.Count >= _times) {

                float? finalResult = GetAverageDeltaTimeResult();
                if (finalResult != null) {

                    if (OptionsHolder.current) {
                        OptionsHolder.current.CurrentOptions.InputTimingCompensation = Mathf.Round((float) finalResult / 0.017f) * 0.017f;
                        onResultApplied?.Invoke();
                    }
                }
            }
        }


        protected void OnNoteStruck (object sender, EventArgs args) {
            if (args is Sheet.NoteStruckEventArgs) {
                Sheet.NoteStruckEventArgs noteStruckEventArgs = (Sheet.NoteStruckEventArgs) args;

                float reversedDeltaTime = -noteStruckEventArgs.deltaTime;

                _reversedDeltaTimeList.Add(reversedDeltaTime);
                onNewResultGenerated?.Invoke(reversedDeltaTime);
            }
        }

        protected void OnNoteMissed (object sender, EventArgs args) {
            _reversedDeltaTimeList.Add(null);
            onNewResultGenerated?.Invoke(null);
        }

        
    }

}