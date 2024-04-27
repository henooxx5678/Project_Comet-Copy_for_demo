using System;
using UnityEngine;
using UnityEngine.Events;
using DoubleHeat.Utilities;
using ProjectComet.Levels;
using ProjectComet.UIDisplay;
using ProjectComet.MenuSceneContents.Display;

namespace ProjectComet.MenuSceneContents {
    
    public class LevelSelectingManager : MonoBehaviour {

        public event EventHandler<EnteredLevelEventArgs> EnteredLevel;

        public UnityEvent onStartToEnterCurrentLevel;
        public UnityEvent onAttemptToEnterLockedStage;
        public UnityEvent onAttemptToEnterLockedHardMode;


        [Header("Info")]
        [SerializeField] int _minStageNumber = 1;
        [SerializeField] int _maxStageNumber = 3;

        LevelDifficulty _currentDifficulty = LevelDifficulty.None;
        public LevelDifficulty CurrentDifficulty {
            get => _currentDifficulty;
            set {
                if (_currentDifficulty != value) {
                    _currentDifficulty = value;
                    OnCurrentDifficultyChanged();
                }
            }
        }

        int _currentStageNumber = 0;
        public int CurrentStageNumber {
            get => _currentStageNumber;
            set {
                if (_currentStageNumber != value) {
                    _currentStageNumber = value;
                    OnCurrentStageNumberChanged();
                }
            }
        }


        [Header("REFS")]
        [SerializeField] LevelSelectingDisplayController _levelSelectingDisplayController;
        [SerializeField] BestLevelResultDisplayController _levelInfoDisplayController;
        [SerializeField] LevelBasicInfoDisplayController _levelBasicInfoDisplayController;

        [SerializeField] MonoBehaviour _levelDifficultyDisplayControllerComponent;
        ILevelDifficultyDisplayController _levelDifficultyDisplayController;
        protected ILevelDifficultyDisplayController levelDifficultyDisplayController {
            get {
                _levelDifficultyDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<ILevelDifficultyDisplayController>( _levelDifficultyDisplayControllerComponent);
                return _levelDifficultyDisplayController;
            }
        }



        const int _defaultStageNumber = 1;
        const LevelDifficulty _defaultLevelDifficulty = LevelDifficulty.Normal;

        protected virtual void Start () {
            CurrentStageNumber = GetCurrentStageNumber();
            CurrentDifficulty = GetCurrentLevelDifficulty();
        }

        protected int GetCurrentStageNumber () {
            string key = "CurrentStageNumber";
            if (PlayerPrefs.HasKey(key)) {
                return PlayerPrefs.GetInt(key);
            }
            return _defaultStageNumber;
        }

        protected LevelDifficulty GetCurrentLevelDifficulty () {
            string key = "CurrentLevelDifficulty";
            if (PlayerPrefs.HasKey(key)) {
                return (LevelDifficulty) PlayerPrefs.GetInt(key);
            }
            return _defaultLevelDifficulty;
        }



        public void AttemptToEnterCurrentLevel () {
            if (!BestRecordsManager.IsStageUnlocked(CurrentStageNumber)) {
                onAttemptToEnterLockedStage?.Invoke();
                return;
            }

            if (CurrentDifficulty == LevelDifficulty.Hard && !BestRecordsManager.IsHardModeUnlocked(CurrentStageNumber)) {
                onAttemptToEnterLockedHardMode?.Invoke();
                return;
            }

            onStartToEnterCurrentLevel?.Invoke();
        }

        public void EnterCurrentLevel () {
            Debug.Log($"Entering Level: Stage {CurrentStageNumber}, {CurrentDifficulty}");
            EnteredLevel?.Invoke(this, new EnteredLevelEventArgs{ stageNumber = CurrentStageNumber, difficulty = CurrentDifficulty });
        }


        public void SwitchStage (int dir) {  // dir: '-1' for previous, '1' for next
            CurrentStageNumber = Math.Max(Math.Min(CurrentStageNumber + dir, _maxStageNumber), _minStageNumber);
        }

        public void SwitchDifficulty () {
            switch (CurrentDifficulty) {
                case LevelDifficulty.Normal: {
                    CurrentDifficulty = LevelDifficulty.Hard;
                    break;
                }
                case LevelDifficulty.Hard: {
                    CurrentDifficulty = LevelDifficulty.Normal;
                    break;
                }
            }
        }

        public void SwitchDifficultyTo (int difficultyNumber) {
            SwitchDifficultyTo((LevelDifficulty) difficultyNumber);
        }

        public void SwitchDifficultyTo (LevelDifficulty difficulty) {
            CurrentDifficulty = difficulty;
        }


        void UpdateLevelInfo () {
            if (_levelInfoDisplayController) {
                _levelBasicInfoDisplayController.InitWithLevel(CurrentStageNumber);
                _levelInfoDisplayController.InitWithIndicatedLevel(CurrentStageNumber, CurrentDifficulty);
            }
        }


        protected virtual void OnCurrentStageNumberChanged() {
            if (_levelSelectingDisplayController) {
                _levelSelectingDisplayController.UpdateStatus(CurrentStageNumber);
            }
            UpdateLevelInfo();

            PlayerPrefs.SetInt("CurrentStageNumber", CurrentStageNumber);
        }

        protected virtual void OnCurrentDifficultyChanged () {
            levelDifficultyDisplayController?.SetSelected(CurrentDifficulty);
            UpdateLevelInfo();

            PlayerPrefs.SetInt("CurrentLevelDifficulty", (int) CurrentDifficulty);
        }



        public class EnteredLevelEventArgs : EventArgs {
            public int stageNumber;
            public LevelDifficulty difficulty;
        }

    }

}
