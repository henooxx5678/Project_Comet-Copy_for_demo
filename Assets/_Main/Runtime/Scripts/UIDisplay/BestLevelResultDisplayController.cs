using System;
using UnityEngine;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {

    public class BestLevelResultDisplayController : BasicLevelResultDisplayController, IBestLevelResultDataHandler {


        public void InitWithIndicatedLevel (int stageNumber, int difficultyNumber) {
            InitWithIndicatedLevel(stageNumber, (LevelDifficulty) difficultyNumber);
        }

        public void InitWithIndicatedLevel (int stageNumber, LevelDifficulty difficulty) {
            InitWithBestResultData(BestRecordsManager.GetBestRecord(stageNumber, difficulty));
        }

        public void InitWithBestResultData (IBestLevelResultData bestResultData) {
            if (bestResultData != null) {
                SetLevelResultRankDisplay(bestResultData.ResultRank);
                SetIsLevelClearedDisplay(bestResultData.IsLevelCleared);

                SetScoreDisplay(bestResultData.Score);
                SetMaxComboDisplay(bestResultData.MaxCombo);

                SetAchievementsDisplay(bestResultData.HasAchievedClear, bestResultData.HasAchievedFullCombo, bestResultData.HasAchievedPerfect);
            }
            else {
                ResetToDefault();
            }

        }

    }
}