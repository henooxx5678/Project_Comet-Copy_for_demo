using System;
using UnityEngine;
using UnityEngine.UI;
using ProjectComet.Levels;

namespace ProjectComet.Debuging {

    
    public class LevelTester : MonoBehaviour {

        public SimpleLevelLauncherForTest levelLauncherForTest;
        public GameObject startingBoard;
        public Text currentDiffShow;


        void OnEnable () {
            levelLauncherForTest.LevelLaunched += OnLevelLaunched;
        }
        void OnDisable () {
            levelLauncherForTest.LevelLaunched -= OnLevelLaunched;
        }


        void OnLevelLaunched (object sender, EventArgs args) {
            startingBoard.SetActive(false);
            currentDiffShow.text = levelLauncherForTest.CurrentDifficulty.ToString();
        }

    }

}