using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.UIDisplay;

namespace ProjectComet.CoreGameplay.Display {

    public class UnpauseCountDownDisplayController : MonoBehaviour, IUnpauseCountDownDisplayController {


        [SerializeField] GameObject _countDownDisplayObject;
        [SerializeField] NumberDisplayControllerWithSprite _countDownNumberDisplayController;


        public void Show () {
            if (_countDownDisplayObject) {
                _countDownDisplayObject.SetActive(true);
            }
        }

        public void Hide () {
            if (_countDownDisplayObject) {
                _countDownDisplayObject.SetActive(false);
            }
        }

        public void UpdateDisplay (int value) {
            _countDownNumberDisplayController.SetCurrentNumber(value);
        }

    }
}