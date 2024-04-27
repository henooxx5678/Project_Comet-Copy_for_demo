using System;
using UnityEngine;
using UnityEngine.UI;
using DoubleHeat.DependsOnAddressableAssets.SceneManagement;
using ProjectComet.Levels;
using ProjectComet.CoreGameplay;
using ProjectComet.MenuSceneContents;

namespace ProjectComet.Global {
    
    public class CrossSceneManager : MonoBehaviour {

        // Messy!!        
        const int _stagesCount = 3;

        const string MAIN_RUNNING_SCENE_SLOT_NAME = "Main";

        [Header("Options")]
        [SerializeField] bool _loadInitSceneAtStart = true;
        [SerializeField] string _startingSceneName;
        [SerializeField] bool _isUsingLoadingScreen;


        [Header("REFS")]
        [SerializeField] ScenesLoadingManager _scenesLoadingManager;
        [SerializeField] GameObject _loadingScreen;


        public string CurrentSceneName {get; private set;} = string.Empty;

        

        protected virtual void OnEnable () {
            if (_scenesLoadingManager) {
                _scenesLoadingManager.SceneSwitchingOfSlotCompleted += OnSwitchedSceneLoaded;
            }
        }

        protected virtual void OnDisable () {
            if (_scenesLoadingManager) {
                _scenesLoadingManager.SceneSwitchingOfSlotCompleted -= OnSwitchedSceneLoaded;
            }
        }


        protected virtual void Start () {
            if (_loadInitSceneAtStart) {
                SwitchMainRunningSceneTo(_startingSceneName, () => InitMenuScene());
            }
        }


        protected void SwitchMainRunningSceneTo (string sceneName, Action initSceneAction = null) {
            if (_isUsingLoadingScreen) {
                _loadingScreen.SetActive(true);
            }

            CurrentSceneName = sceneName;
            _scenesLoadingManager.SwtichSceneOfSlotTo(MAIN_RUNNING_SCENE_SLOT_NAME, sceneName, initSceneAction);
        }


        protected void InitMenuScene (string stageName = null) {
            if (!string.IsNullOrEmpty(stageName)) {
                MenuSceneManager menuSceneManager = FindObjectOfType<MenuSceneManager>(true);

                if (menuSceneManager) {
                    menuSceneManager.SetCurrentStage(stageName);
                }
                else {
                    Debug.LogError("Could not find MenuSceneManager!");
                }
            }

            LevelSelectingManager levelSelectingManager = FindObjectOfType<LevelSelectingManager>(true);
            if (levelSelectingManager) {
                levelSelectingManager.EnteredLevel += OnTryToEnterTargetLevel;
            }
            else {
                Debug.LogError("Could not find LevelSelectingManager!");
            }
        }

        protected void InitGamePlayScene (int stageNumber, LevelDifficulty difficulty) {

            PlayerPrefs.SetInt("CurrentStageNumber", stageNumber);
            PlayerPrefs.SetInt("CurrentLevelDifficulty", (int) difficulty);


            LevelLauncher levelLauncher = FindObjectOfType<LevelLauncher>();

            if (levelLauncher) {
                levelLauncher.StartLaunching(stageNumber, difficulty);
            }
            else {
                Debug.LogError("Could not find LevelLauncher!");
            }

            LevelPlayingStatesManager levelPlayingStatesManager = FindObjectOfType<LevelPlayingStatesManager>(true);
            if (levelPlayingStatesManager) {
                levelPlayingStatesManager.ExitLevelAction = OnBackToLevelSelecting;
                levelPlayingStatesManager.ExitLevelAndNextAction = OnBackToLevelSelectingAndNext;
            }
        }

  


        protected virtual void OnSwitchedSceneLoaded (object sender, EventArgs args) {

            if (args is ScenesLoadingManager.SceneEventArgs) {
                ScenesLoadingManager.SceneEventArgs sceneEventArgs = (ScenesLoadingManager.SceneEventArgs) args;

                if (sceneEventArgs.sceneName == CurrentSceneName) {
                    if (_loadingScreen) {
                        _loadingScreen.SetActive(false);
                    }
                }
                // switch (sceneEventArgs.sceneName) {
                    
                //     case "Menu": {
                //         break;
                //     }
                //     case "Game Play": {
                //         break;
                //     }
                // }
            }
        }

        void OnTryToEnterTargetLevel (object sender, LevelSelectingManager.EnteredLevelEventArgs args) {
            SwitchMainRunningSceneTo("Game Play", () => InitGamePlayScene(args.stageNumber, args.difficulty) );
        }

        void OnBackToMenuBeginning (object sender, EventArgs args) {
            SwitchMainRunningSceneTo("Menu", () => InitMenuScene("Main Menu"));
        }

        void OnBackToLevelSelecting () {
            SwitchMainRunningSceneTo("Menu", () => InitMenuScene("Level Selecting"));
        }

        void OnBackToLevelSelectingAndNext () {
            string key = "CurrentStageNumber";
            if (PlayerPrefs.HasKey(key)) {
                int nextStageNumber = Math.Min(PlayerPrefs.GetInt(key) + 1, _stagesCount);
                PlayerPrefs.SetInt(key, nextStageNumber);
            }

            OnBackToLevelSelecting();
        }



        public enum SceneType {
            None,
            Menu,
            GamePlay
        }

    }

}