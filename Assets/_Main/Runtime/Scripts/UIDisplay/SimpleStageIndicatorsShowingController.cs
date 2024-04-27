using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {

    public class SimpleStageIndicatorsShowingController : MonoBehaviour, IInitableWithLevel {

        [SerializeField] StageIndicatorsInfo[] _stageIndicatorsInfos;

        public void InitWithLevel (int stageNumber, LevelDifficulty difficulty) {
            
            foreach (StageIndicatorsInfo info in _stageIndicatorsInfos) {
                info?.SetIndicatorsActive(info.StageNumber == stageNumber);
            }

        }


        [Serializable]
        public class StageIndicatorsInfo {

            public int StageNumber => _stageNumber;

            [SerializeField] int _stageNumber;
            [SerializeField] GameObject[] _indicators;

            public void SetIndicatorsActive (bool active) {
                foreach (GameObject indicator in _indicators) {
                    indicator.SetActive(active);
                }
            }
        }

    }
}