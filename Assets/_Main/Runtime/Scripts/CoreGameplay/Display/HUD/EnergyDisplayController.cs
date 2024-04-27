using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.CoreGameplay.ScoringStatus;

namespace ProjectComet.CoreGameplay.Display {

    public class EnergyDisplayController : MonoBehaviour, IEnergyDisplayController {


        [Header("REFS")]
        [SerializeField] GameObject _energyBarObject;
        [SerializeField] Image _energyBarImage;

        public void Show () {
            if (_energyBarObject) {
                _energyBarObject.SetActive(true);
            }
        }

        public void Hide () {
            if (_energyBarObject) {
                _energyBarObject.SetActive(false);
            }
        }

        public void UpdateDisplay (float energyValue) {
            if (_energyBarImage) {
                _energyBarImage.fillAmount = Mathf.Clamp(energyValue, 0f, 1f);
            }
        }

    }
}