using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DoubleHeat.UI;
using ProjectComet.CoreGameplay.ScoringStatus;

namespace ProjectComet.CoreGameplay.Display {

    public class ScoreDisplayController : MonoBehaviour, IScoreDisplayController {

        [Header("REFS")]
        [SerializeField] GameObject _displayObject;
        [SerializeField] Component _valueTextDisplay;

        public UnityEvent<int> onUpdateDisplay;


        public void Show() {
            if (_displayObject) {
                _displayObject.SetActive(true);
            }
        }

        public void Hide() {
            if (_displayObject) {
                _displayObject.SetActive(false);
            }
        }

        public void UpdateDisplay(int score) {
            TextDisplayTools.SetTextOfTextDisplayComponent(_valueTextDisplay, score.ToString());
            onUpdateDisplay?.Invoke(score);
        }

    }
}