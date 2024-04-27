using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DoubleHeat.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {

    public abstract class BasicLevelResultDisplayController : MonoBehaviour {

        [SerializeField]
        LevelResultDisplayContent _defaultDisplayContent;


        [Header("REFS")]
        [SerializeField] LevelResultRankDisplayController _levelResultRankDisplayController;
        [SerializeField] GameObject _clearedShowingObject;
        [SerializeField] AchievementDisplayController _achievementClearDisplayController;
        [SerializeField] AchievementDisplayController _achievementFullComboDisplayController;
        [SerializeField] AchievementDisplayController _achievementPerfectDisplayController;
        [SerializeField] Component _scoreTextDisplay;
        [SerializeField] Component _maxComboTextDisplay;
        [SerializeField] Component _perfectCountTextDisplay;
        [SerializeField] Component _goodCountTextDisplay;
        [SerializeField] Component _missCountTextDisplay;
        [SerializeField] Component _safeCountTextDisplay;
        [SerializeField] Component _failCountTextDisplay;




        protected static void SetTextDisplayAsIntValue(Component textDisplayComponent, int value) {
            TextDisplayTools.SetTextOfTextDisplayComponent(textDisplayComponent, value.ToString());
        }


        protected void ResetToDefault () {
            SetLevelResultRankDisplay(_defaultDisplayContent.ResultRank);
            SetIsLevelClearedDisplay(_defaultDisplayContent.IsCleared);
            SetAchievementsDisplay(_defaultDisplayContent.HasAchievedClear, _defaultDisplayContent.HasAchievedFullCombo, _defaultDisplayContent.HasAchievedPerfect);

            TextDisplayTools.SetTextOfTextDisplayComponent(_scoreTextDisplay, _defaultDisplayContent.ScoreDisplayText);
            TextDisplayTools.SetTextOfTextDisplayComponent(_maxComboTextDisplay, _defaultDisplayContent.MaxComboDisplayText);

            TextDisplayTools.SetTextOfTextDisplayComponent(_perfectCountTextDisplay, _defaultDisplayContent.PerfectCountDisplayText);
            TextDisplayTools.SetTextOfTextDisplayComponent(_goodCountTextDisplay, _defaultDisplayContent.GoodCountDisplayText);
            TextDisplayTools.SetTextOfTextDisplayComponent(_missCountTextDisplay, _defaultDisplayContent.MissCountDisplayText);
            TextDisplayTools.SetTextOfTextDisplayComponent(_safeCountTextDisplay, _defaultDisplayContent.SafeCountDisplayText);
            TextDisplayTools.SetTextOfTextDisplayComponent(_failCountTextDisplay, _defaultDisplayContent.FailCountDisplayText);

    
        }


        protected void SetLevelResultRankDisplay(LevelResultRank rank) {
            if (_levelResultRankDisplayController) {
                _levelResultRankDisplayController.ShowRank(rank);
            }
        }

        protected void SetIsLevelClearedDisplay(bool isLevelCleared) {
            if (_clearedShowingObject) {
                _clearedShowingObject.SetActive(isLevelCleared);
            }
        }

        protected void SetScoreDisplay (int score) {
            SetTextDisplayAsIntValue(_scoreTextDisplay, score);
        }

        protected void SetMaxComboDisplay (int maxCombo) {
            SetTextDisplayAsIntValue(_maxComboTextDisplay, maxCombo);
        }

        protected void SetPerfectCountDisplay (int count) {
            SetTextDisplayAsIntValue(_perfectCountTextDisplay, count);
        }

        protected void SetGoodCountDisplay (int count) {
            SetTextDisplayAsIntValue(_goodCountTextDisplay, count);
        }

        protected void SetMissCountDisplay (int count) {
            SetTextDisplayAsIntValue(_missCountTextDisplay, count);
        }

        protected void SetSafeCountDisplay (int count) {
            SetTextDisplayAsIntValue(_safeCountTextDisplay, count);
        }

        protected void SetFailCountDisplay (int count) {
            SetTextDisplayAsIntValue(_failCountTextDisplay, count);
        }

        protected void SetAchievementsDisplay(bool clear, bool fullCombo, bool perfect) {
            if (_achievementClearDisplayController) {
                _achievementClearDisplayController.HasAchieved = clear;
            }
            if (_achievementFullComboDisplayController) {
                _achievementFullComboDisplayController.HasAchieved = fullCombo;
            }
            if (_achievementPerfectDisplayController) {
                _achievementPerfectDisplayController.HasAchieved = perfect;
            }
        }




    }
}