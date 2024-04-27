using UnityEngine;

namespace ProjectComet.Levels {

    public class SimpleLevelLauncherForTest : LevelLauncher {

        [Header("Level Selecting")]
        [SerializeField] bool _launchAtStart = false;
        
        [Range(0, 3)]
        [SerializeField] int _levelNumber = 1;
        [SerializeField] LevelDifficulty _currentDifficulty = LevelDifficulty.Hard;
        public LevelDifficulty CurrentDifficulty => _currentDifficulty;
    


        protected virtual void Start () {
            if (_launchAtStart) {
                Launch();
            }
        }


        public void Launch () {
            StartLaunching(_levelNumber, _currentDifficulty);
        }

        public void TestLaunchWithDiff (int diffNum) {
            Launch ((LevelDifficulty) diffNum);
        }

        public void TestLauchWithStage (int stageNum) {
            _levelNumber = stageNum;
            Launch();
        }

        public void Launch (LevelDifficulty difficulty) {
            _currentDifficulty = difficulty;
            Launch();
        }


    }
}