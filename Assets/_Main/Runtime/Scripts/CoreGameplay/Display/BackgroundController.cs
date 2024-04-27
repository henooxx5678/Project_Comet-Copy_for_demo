using System;
using UnityEngine;
using ProjectComet.Levels;

namespace ProjectComet.CoreGameplay.Display {

    public class BackgroundController : MonoBehaviour {

        [SerializeField] LevelLauncher _levelLauncher;
        [SerializeField] GameObject[] _bgs;


        void OnEnable () {
            if (_levelLauncher) {
                _levelLauncher.LevelLaunched += OnLevelLaunched;
            }
            else {
                Debug.LogWarning("no _levelLauncher");
            }
        }

        void OnDisable () {
            if (_levelLauncher) {
                _levelLauncher.LevelLaunched -= OnLevelLaunched;
            }
        }

        public void SetBackgroundByStageNumber (int stageNubmer) {
            for (int i = 0 ; i < _bgs.Length ; i++) {
                
                if (_bgs[i]) {
                    if (i == stageNubmer - 1) {
                        _bgs[i].SetActive(true);
                    }
                    else {
                        _bgs[i].SetActive(false);
                    }
                }
            }
        }

        void OnLevelLaunched (object sender, EventArgs args) {
            if (args is LevelLauncher.LevelLaunchedEventArgs) {
                LevelLauncher.LevelLaunchedEventArgs levelLaunchedEventArgs = (LevelLauncher.LevelLaunchedEventArgs) args;

                SetBackgroundByStageNumber(levelLaunchedEventArgs.stageNumber);
            }
        }

    }

}