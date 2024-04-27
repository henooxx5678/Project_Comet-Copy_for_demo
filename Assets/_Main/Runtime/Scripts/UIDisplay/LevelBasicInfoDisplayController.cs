using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Infos;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {
    
    public class LevelBasicInfoDisplayController : MonoBehaviour, IInitableWithLevel {

        const string lockedStageName = "???";
        const string lockedComposerName = "???";

        [SerializeField] Text _stageNameTextDisplay;
        [SerializeField] Text _composerTextDisplay;
        [SerializeField] Text _prevStageNameForPopupWarningTextDisplay;

        public void InitWithLevel (int stageNumber, LevelDifficulty difficulty = LevelDifficulty.None) {
            if (StagesBasicInfoManager.current) {

                if (BestRecordsManager.IsStageUnlocked(stageNumber)) {

                    StageBasicInfo info = StagesBasicInfoManager.current.GetStageBasicInfoByStageNumber(stageNumber);

                    if (info != null) {
                        if (_stageNameTextDisplay) {
                            _stageNameTextDisplay.text = info.Name;
                        }
                        if (_composerTextDisplay) {
                            _composerTextDisplay.text = info.Composer;
                        }
                    }
                    else {
                        Debug.LogWarning("Stage info not found!");
                    }
                }
                else {
                    if (_stageNameTextDisplay) {
                        _stageNameTextDisplay.text = lockedStageName;
                    }
                    if (_composerTextDisplay) {
                        _composerTextDisplay.text = lockedComposerName;
                    }
                }

                StageBasicInfo prevStageInfo = StagesBasicInfoManager.current.GetStageBasicInfoByStageNumber(stageNumber - 1);
                if (prevStageInfo != null) {
                    if (_prevStageNameForPopupWarningTextDisplay) {
                        _prevStageNameForPopupWarningTextDisplay.text = $"\"{prevStageInfo.Name}\".";
                    }
                }
            }
            else {
                Debug.LogWarning("StagesBasicInfoManager not found!");
            }
        }
    }

}