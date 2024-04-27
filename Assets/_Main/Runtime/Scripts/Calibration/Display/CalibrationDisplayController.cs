using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ProjectComet.CoreGameplay;
using ProjectComet.CoreGameplay.Notes;
using ProjectComet.Levels;

namespace ProjectComet.Calibration {

    public class CalibrationDisplayController : MonoBehaviour {

        public UnityEvent onFinalResultShowed;

        [SerializeField] float _resultShowingWaitingTime = 1f;

        [Header("REFS")]
        [SerializeField] CalibrationResultProvider _resultProvider;
        [SerializeField] Text[] _resultShowingTextDisplays;
        [SerializeField] GameObject _finalResultPanel;
        [SerializeField] Text _finalResultShowingTextDisplay;


        int _currentWaitingResultSlotIndex = 0;


        protected virtual void OnEnable () {
            _currentWaitingResultSlotIndex = 0;
            foreach (Text textDisplay in _resultShowingTextDisplays) {
                textDisplay.text = string.Empty;
            }
        }


        public void AddNewResult (float? resultValue) {
            if (_currentWaitingResultSlotIndex >= 0 && _currentWaitingResultSlotIndex < _resultShowingTextDisplays.Length) {

                _resultShowingTextDisplays[_currentWaitingResultSlotIndex].text = GetResultText(resultValue);
                _currentWaitingResultSlotIndex++;
            }

            if (_currentWaitingResultSlotIndex >= _resultShowingTextDisplays.Length) {
                ReadyToShowFinalResult();
            }
        }

        
        void ReadyToShowFinalResult () {
            StartCoroutine(WaitingForShowingFinalResult(_resultProvider.GetAverageDeltaTimeResult()));
        }

        IEnumerator WaitingForShowingFinalResult (float? resultValue) {

            yield return new WaitForSecondsRealtime(_resultShowingWaitingTime);

            _finalResultPanel.SetActive(true);
            _finalResultShowingTextDisplay.text = GetResultText(resultValue);

            onFinalResultShowed?.Invoke();
        }


        protected string GetResultText (float? value) {
            return value != null ? (-(int) Mathf.Round((float) value / 0.017f)).ToString() : "-";
        }
        
    }

}