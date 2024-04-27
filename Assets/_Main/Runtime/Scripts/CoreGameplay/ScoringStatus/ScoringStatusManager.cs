using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoubleHeat.Common;
using DoubleHeat.Utilities;
using ProjectComet.Levels;


namespace ProjectComet.CoreGameplay.ScoringStatus {

    public class ScoringStatusManager : MonoBehaviour {

        int _currentScore = -1;
        public int CurrentScore {
            get => _currentScore;
            set {
                if (_currentScore != value) {
                    _currentScore = value;
                    OnCurrentScoreChanged(_currentScore);
                }
            }
        }

        float _currentEnergyWeight = -1f;
        protected float currentEnergyWeight {
            get => _currentEnergyWeight;
            set {
                _currentEnergyWeight = value;
                CurrentEnergy = _currentEnergyWeight / (float) _totalNotesCount;
            }
        }

        float _currentEnergy = -1f;
        public float CurrentEnergy {
            get => _currentEnergy;
            set {
                if (_currentEnergy != value) {
                    _currentEnergy = value;
                    OnCurrentEnergyChanged(_currentEnergy);
                }
            }
        }

        int _currentCombo = -1;
        public int CurrentCombo {
            get => _currentCombo;
            set {
                if (_currentCombo != value) {
                    _currentCombo = value;
                    OnCurrentComboChanged(_currentCombo);
                }
            }
        }

        int _currentMaxCombo = -1;
        public int CurrentMaxCombo {
            get => _currentMaxCombo;
            set {
                if (_currentMaxCombo != value) {
                    _currentMaxCombo = value;
                    OnCurrentMaxComboChanged(_currentMaxCombo);
                }
            }
        }

        public ResultTypesCounter CurrentResultTypesCounter {get; private set;}


        [Header("Settings")]
        [SerializeField] int _minComboValueToShow = 3;
        [SerializeField] AffectOfResult[] _affectOfResults;
        [SerializeField] ComboCoefficientInfo[] _comboCoefficientInfos;
        [SerializeField] LevelClearThresholdOfDifficulty[] _levelClearThresholdOfDifficulties;
        [SerializeField] LevelResultRankThreshold[] _levelResultRankThreshold;


        [Header("REFS")]
        [SerializeField] MonoBehaviour _scoreDisplayControllerComponent;
        IScoreDisplayController _scoreDisplayController;
        protected IScoreDisplayController scoreDisplayController {
            get {
                // if (_scoreDisplayController == null && _scoreDisplayControllerComponent is IScoreDisplayController) {
                //     _scoreDisplayController = (IScoreDisplayController) _scoreDisplayControllerComponent;
                // }
                _scoreDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<IScoreDisplayController>(_scoreDisplayControllerComponent);
                return _scoreDisplayController;
            }
        }

        [SerializeField] MonoBehaviour _energyDisplayControllerComponent;
        IEnergyDisplayController _energyDisplayController;
        protected IEnergyDisplayController energyDisplayController {
            get {
                // if (_energyDisplayController == null && _energyDisplayControllerComponent is IEnergyDisplayController) {
                //     _energyDisplayController = (IEnergyDisplayController) _energyDisplayControllerComponent;
                // }
                _energyDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<IEnergyDisplayController>(_energyDisplayControllerComponent);
                return _energyDisplayController;
            }
        }

        [SerializeField] MonoBehaviour _comboDisplayControllerComponent;
        IScoreDisplayController _comboDisplayController;
        protected IScoreDisplayController comboDisplayController {
            get {
                // if (_comboDisplayController == null && _comboDisplayControllerComponent is IScoreDisplayController) {
                //     _comboDisplayController = (IScoreDisplayController)_comboDisplayControllerComponent;
                // }
                _comboDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<IScoreDisplayController>(_comboDisplayControllerComponent);
                return _comboDisplayController;
            }
        }

        [SerializeField] MonoBehaviour _maxComboDisplayControllerComponent;
        IScoreDisplayController _maxComboDisplayController;
        protected IScoreDisplayController maxComboDisplayController {
            get {
                // if (_maxComboDisplayController == null && _maxComboDisplayControllerComponent is IScoreDisplayController) {
                //     _maxComboDisplayController = (IScoreDisplayController)_maxComboDisplayControllerComponent;
                // }
                _maxComboDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<IScoreDisplayController>((MonoBehaviour) _maxComboDisplayControllerComponent);
                return _maxComboDisplayController;
            }
        }



        int _totalNotesCount = -1;
        float _levelClearEnergyThreshold = 0f;


        public static ResultType ConvertToResultTypeFromStrikeResult(StrikeResult strikeResult) {
            switch (strikeResult) {
                case StrikeResult.Perfect: {
                    return ResultType.Perfect;
                }
                case StrikeResult.Good: {
                    return ResultType.Good;
                }
                default: {
                    return ResultType.None;
                }
            }
        }

        public static ResultType ConvertToResultTypeFromMonsterPassResult(MonsterPassResult monsterPassResult) {
            switch (monsterPassResult) {
                case MonsterPassResult.Safe: {
                    return ResultType.Safe;
                }
                case MonsterPassResult.Fail: {
                    return ResultType.Fail;
                }
                default: {
                    return ResultType.None;
                }
            }
        }


        
        
        public void Init (int totalNotesCount, LevelDifficulty difficulty) {
            currentEnergyWeight = 0f;
            CurrentCombo = 0;
            CurrentMaxCombo = 0;
            CurrentScore = 0;
            _totalNotesCount = totalNotesCount;
            _levelClearEnergyThreshold = LevelClearThresholdOfDifficulty.GetEnergyThreshold(_levelClearThresholdOfDifficulties, difficulty);
            CurrentResultTypesCounter = new ResultTypesCounter(OnResultTypeCountChanged);
        }

        public LevelResultData GetLevelResultDataWhenEnded () {
            bool isCleared = CurrentEnergy >= _levelClearEnergyThreshold;

            LevelResultRank rank = isCleared ? LevelResultRankThreshold.GetTargetRank(_levelResultRankThreshold, CurrentEnergy) : LevelResultRank.F;

            return new LevelResultData (
                rank,
                isCleared,
                CurrentScore,
                CurrentMaxCombo,
                CurrentResultTypesCounter.GetCountOf(ResultType.Perfect),
                CurrentResultTypesCounter.GetCountOf(ResultType.Good),
                CurrentResultTypesCounter.GetCountOf(ResultType.Miss),
                CurrentResultTypesCounter.GetCountOf(ResultType.Safe),
                CurrentResultTypesCounter.GetCountOf(ResultType.Fail)
            );
        }



        public void ApplyAffect (ResultType resultType) {
            ApplyAffect(GetAffectOfResultByResultType(resultType));
        }


        protected void ApplyAffect (AffectOfResult affect) {
            if (affect != null) {
                currentEnergyWeight += affect.energyGainRate;
                CurrentCombo = affect.willKeepCombo ? CurrentCombo + 1 : 0;
                CurrentMaxCombo = Math.Max(CurrentMaxCombo, CurrentCombo);
                CurrentScore += (int) (affect.baseScoring * ComboCoefficientInfo.GetTargetComboCoefficient(_comboCoefficientInfos, CurrentCombo));

                CurrentResultTypesCounter.Add(affect.resultType);
            }
        }
        

        protected AffectOfResult GetAffectOfResultByResultType (ResultType resultType) {
            return AffectOfResult.GetFromArrayByResultType(_affectOfResults, resultType);
        }



        void OnCurrentScoreChanged (int newScoreValue) {
            scoreDisplayController?.UpdateDisplay(newScoreValue);
        }

        void OnCurrentEnergyChanged (float newEnergyValue) {
            energyDisplayController?.UpdateDisplay(newEnergyValue);
        }

        void OnCurrentComboChanged (int newComboValue) {
            if (newComboValue < _minComboValueToShow) {
                comboDisplayController?.Hide();
            }
            else {
                comboDisplayController?.Show();
                comboDisplayController?.UpdateDisplay(newComboValue);
            }
        }

        void OnCurrentMaxComboChanged (int newMaxComboValue) {
            maxComboDisplayController?.UpdateDisplay(newMaxComboValue);
        }


        void OnResultTypeCountChanged (ResultType resultType, int count) {
            
        }




        public class ResultTypesCounter {
            Dictionary<ResultType, int> _countOfStrikeResults = new Dictionary<ResultType, int>();


            Action<ResultType, int> _onStrikeResultCountChanged = null;


            public ResultTypesCounter (Action<ResultType, int> onStrikeResultCountChanged) {
                _onStrikeResultCountChanged = onStrikeResultCountChanged;
            }


            public void ClearAll () {
                _countOfStrikeResults.Clear();

                foreach (ResultType resultType in _countOfStrikeResults.Keys) {
                    _onStrikeResultCountChanged?.Invoke(resultType, GetCountOf(resultType));
                }
            }

            public void Add (ResultType resultType, int count = 1) {
                if (_countOfStrikeResults.ContainsKey(resultType)) {
                    _countOfStrikeResults[resultType] += count;
                }
                else {
                    _countOfStrikeResults.Add(resultType, count);
                }

                _onStrikeResultCountChanged?.Invoke(resultType, GetCountOf(resultType));
            }

            public int GetCountOf (ResultType resultType) {
                if (_countOfStrikeResults.ContainsKey(resultType)) {
                    return _countOfStrikeResults[resultType];
                }
                return 0;
            }
        }

        [Serializable]
        protected class AffectOfResult {
            [SerializeField] ResultType _resultType;
            public ResultType resultType => _resultType;
            [SerializeField] int _baseScoring;
            public int baseScoring => _baseScoring;
            [SerializeField] float _energyGainRate;
            public float energyGainRate => _energyGainRate;

            public bool willKeepCombo;

            public static AffectOfResult GetFromArrayByResultType (AffectOfResult[] array, ResultType resultType) {
                return Array.Find(array, elem => elem.resultType == resultType);
            }
        }

        [Serializable]
        protected class ComboCoefficientInfo {
            [SerializeField] IntRange _currentComboRange;
            public IntRange CurrentComboRange => _currentComboRange;
            [SerializeField] float _comboCoefficient;
            public float ComboCoefficient => _comboCoefficient;

            public static float GetTargetComboCoefficient (IEnumerable<ComboCoefficientInfo> infos, int currentCombo) {

                ComboCoefficientInfo info = infos.FirstOrDefault(info => info.CurrentComboRange.IsInRange(currentCombo));

                if (info != null) {
                    return info.ComboCoefficient;
                }
                return 1;
            }
        }


        [Serializable]
        protected struct LevelClearThresholdOfDifficulty {
            [SerializeField] float _energyThreshold;
            [SerializeField] LevelDifficulty _difficulty;

            public float EnergyThreshold => _energyThreshold;
            public LevelDifficulty Difficulty => _difficulty;

            public static float GetEnergyThreshold (IEnumerable<LevelClearThresholdOfDifficulty> levelClearThresholdOfDifficulties, LevelDifficulty difficulty) {
                foreach (var thresholdInfo in levelClearThresholdOfDifficulties) {
                    if (thresholdInfo.Difficulty == difficulty) {
                        return thresholdInfo.EnergyThreshold;
                    }
                }
                return -1f;
            }
        }

        [Serializable]
        protected struct LevelResultRankThreshold {
            [SerializeField] float _energyThreshold;
            [SerializeField] LevelResultRank _rank;

            public float EnergyThreshold => _energyThreshold;
            public LevelResultRank Rank => _rank;

            public static LevelResultRank GetTargetRank (IEnumerable<LevelResultRankThreshold> rankThresholds, float energy) {
                float highestThreshold = 0f;
                LevelResultRank rank = LevelResultRank.F;
                
                foreach (var rankThreshold in rankThresholds) {
                    if (energy >= rankThreshold.EnergyThreshold && rankThreshold.EnergyThreshold > highestThreshold) {
                        rank = rankThreshold.Rank;
                    }
                }
                
                return rank;
            }
        }
       

        public enum ResultType {
            None,
            Perfect,
            Good,
            Miss,
            Safe,
            Fail
        }


    }
}