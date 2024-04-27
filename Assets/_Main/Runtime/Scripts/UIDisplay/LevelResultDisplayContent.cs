using UnityEngine;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {

    [CreateAssetMenu(fileName = "LevelResultDisplayContent", menuName = "ScriptableObjects/LevelResultDisplayContent")]
    public class LevelResultDisplayContent : ScriptableObject {
        
        [SerializeField] LevelResultRank _resultRank;
        public LevelResultRank ResultRank => _resultRank;

        [SerializeField] bool _isCleared;
        public bool IsCleared => _isCleared;

        [SerializeField] string _scoreDisplayText;
        public string ScoreDisplayText => _scoreDisplayText;

        [SerializeField] string _maxComboDisplayText;
        public string MaxComboDisplayText => _maxComboDisplayText;

        [SerializeField] bool _hasAchievedClear;
        public bool HasAchievedClear => _hasAchievedClear;

        [SerializeField] bool _hasAchievedFullCombo;
        public bool HasAchievedFullCombo => _hasAchievedFullCombo;

        [SerializeField] bool _hasAchievedPerfect;
        public bool HasAchievedPerfect => _hasAchievedPerfect;


        [SerializeField] string _perfectCountDisplayText;
        public string PerfectCountDisplayText => _perfectCountDisplayText;

        [SerializeField] string _goodCountDisplayText;
        public string GoodCountDisplayText => _goodCountDisplayText;

        [SerializeField] string _missCountDisplayText;
        public string MissCountDisplayText => _missCountDisplayText;

        [SerializeField] string _safeCountDisplayText;
        public string SafeCountDisplayText => _safeCountDisplayText;
        
        [SerializeField] string _failCountDisplayText;
        public string FailCountDisplayText => _failCountDisplayText;

    }


}