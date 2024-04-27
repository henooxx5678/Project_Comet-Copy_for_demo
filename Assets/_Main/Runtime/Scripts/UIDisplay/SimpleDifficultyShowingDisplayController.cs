using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {
    
    public class SimpleDifficultyShowingDisplayController : MonoBehaviour, IInitableWithLevel {
        
        [SerializeField] GameObject _normalDifficultyDisplay;
        [SerializeField] GameObject _hardDifficultyDisplay;

        public void InitWithLevel (int stageNumber, LevelDifficulty difficulty) {
            switch (difficulty) {
                case LevelDifficulty.Normal: {
                    _normalDifficultyDisplay.SetActive(true);
                    _hardDifficultyDisplay.SetActive(false);
                    break;
                }
                case LevelDifficulty.Hard: {
                    _normalDifficultyDisplay.SetActive(false);
                    _hardDifficultyDisplay.SetActive(true);
                    break;
                }
            }
        }

    }

}