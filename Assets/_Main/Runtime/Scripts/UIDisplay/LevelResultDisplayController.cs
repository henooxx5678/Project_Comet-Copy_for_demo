using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {

    public class LevelResultDisplayController : BasicLevelResultDisplayController, ILevelResultDataHandler {
        
        [SerializeField] Component _bestScoreTextDisplay;


        public void InitWithLevelResultData (LevelResultData resultData, int bestScore) {
            if (resultData != null) {

                SetLevelResultRankDisplay(resultData.ResultRank);
                SetIsLevelClearedDisplay(resultData.IsLevelCleared);

                SetScoreDisplay(resultData.Score);
                SetMaxComboDisplay(resultData.MaxCombo);
                SetPerfectCountDisplay(resultData.PerfectCount);
                SetGoodCountDisplay(resultData.GoodCount);
                SetMissCountDisplay(resultData.MissCount);
                SetSafeCountDisplay(resultData.SafeCount);
                SetFailCountDisplay(resultData.FailCount);
                
                SetAchievementsDisplay(resultData.HasAchievedClear, resultData.HasAchievedFullCombo, resultData.HasAchievedPerfect);

            }

            if (resultData.Score > bestScore) {
                bestScore = resultData.Score;
                OnRefreshBestScore();
            }

            SetTextDisplayAsIntValue(_bestScoreTextDisplay, bestScore);
        }


        protected virtual void OnRefreshBestScore () {
            // TODO: anim
        }

    }

}