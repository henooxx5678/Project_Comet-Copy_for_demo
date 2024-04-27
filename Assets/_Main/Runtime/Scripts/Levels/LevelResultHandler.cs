using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay;
using ProjectComet.CoreGameplay.ScoringStatus;
using ProjectComet.StoryShowing;


namespace ProjectComet.Levels {

    public class LevelResultHandler : MonoBehaviour {

        [Header("Stories")]
        [SerializeField] AssetReference[] _storyPrefabAssetRefs;

        [Header("REFS")]
        [SerializeField] LevelLauncher _levelLauncher;
        [SerializeField] LevelPlayingStatesManager _levelPlayingStatusManager;
        [SerializeField] ScoringStatusManager _levelScoringStatusManager;


        [Header("Display Objects")]
        [SerializeField] GameObject _levelResultDisplayObject;
        [SerializeField] MonoBehaviour _levelResultDisplayControllerComponent;
        ILevelResultDataHandler _levelResultDisplayController;
        protected ILevelResultDataHandler levelResultDisplayController {
            get {
                _levelResultDisplayController ??= MonoBehaviourTools.GetConvertedMonoBehaviour<ILevelResultDataHandler>(_levelResultDisplayControllerComponent);
                return _levelResultDisplayController;
            }
        }


        IBestLevelResultData _bestRecord = null;
        int _currentStageNumber = -1;
        LevelDifficulty _currentDifficulty = LevelDifficulty.None;
        

        protected virtual void OnEnable () {
            if (_levelLauncher) {
                _levelLauncher.LevelLaunched += OnLevelLaunched;
            }
            if (_levelPlayingStatusManager) {
                _levelPlayingStatusManager.LevelStarted += OnLevelStarted;
                _levelPlayingStatusManager.LevelEnded += OnLevelEnded;
            }
        }

        protected virtual void OnDisable () {
            if (_levelLauncher) {
                _levelLauncher.LevelLaunched -= OnLevelLaunched;
            }
            if (_levelPlayingStatusManager) {
                _levelPlayingStatusManager.LevelStarted -= OnLevelStarted;
                _levelPlayingStatusManager.LevelEnded -= OnLevelEnded;
            }
        }

        public void Init (IBestLevelResultData bestRecord) {
            _bestRecord = bestRecord;
        }
                


        protected virtual void OnLevelLaunched (object sender, EventArgs args) {
            if (args is LevelLauncher.LevelLaunchedEventArgs) {
                LevelLauncher.LevelLaunchedEventArgs levelLaunchedEventArgs = (LevelLauncher.LevelLaunchedEventArgs) args;
                _currentStageNumber = levelLaunchedEventArgs.stageNumber;
                _currentDifficulty = levelLaunchedEventArgs.difficulty;
            }
        }

        protected virtual void OnLevelStarted (object sender, EventArgs args) {
            if (_levelResultDisplayObject) {
                _levelResultDisplayObject.SetActive(false);
            }
        }

        [ContextMenu("OnLevelEnded Without Save")]
        void OnLevelEndedWithoutSave () {
            OnLevelEnded(_levelScoringStatusManager.GetLevelResultDataWhenEnded(), false);
        }

        [ContextMenu("OnLevelEnded Test With Clear")]
        void OnLevelEndedTestWithClear () {
            OnLevelEnded(_levelScoringStatusManager.GetLevelResultDataWhenEnded(), false, true);
        }

        protected virtual void OnLevelEnded (object sender, EventArgs args) {

            LevelResultData data = null;

            if (_levelScoringStatusManager) {
                data = _levelScoringStatusManager.GetLevelResultDataWhenEnded();
            }

            OnLevelEnded(data, true);
        }

        protected virtual void OnLevelEnded (LevelResultData data, bool saveResult, bool forceShowStory = false) {

            Debug.Log("Level result: " + data);

            if (saveResult) {
                BestRecordsManager.SaveToBestRecord(_currentStageNumber, _currentDifficulty, data);
            }

            if (levelResultDisplayController != null) {
                int bestScore = _bestRecord != null ? _bestRecord.Score : 0;
                levelResultDisplayController.InitWithLevelResultData(data, bestScore);
            } 
            else {
                Debug.LogWarning("Cannot get result because \"levelResultDisplayController\" is empty.");
            }


            if (data.IsLevelCleared || forceShowStory) {
                ShowStory();
            } 
            else {
                OnReadyToShowResult();
            }

        }

        protected void OnReadyToShowResult () {
            if (_levelResultDisplayObject) {
                _levelResultDisplayObject.SetActive(true);
            }
            else {
                Debug.LogWarning("\"_levelResultDisplayObject\" is empty.");
            }
            
        }

        protected void ShowStory () {
            AssetReference targetStoryPrefabAssetRef = GetStoryPrefabAssetRefByStageNumber(_currentStageNumber);

            if (targetStoryPrefabAssetRef != null) {
                Addressables.InstantiateAsync(targetStoryPrefabAssetRef, this.transform).Completed += OnStoryInstantiated;
            }
            else {
                Debug.LogWarning("No story prefab asset reference found!");
            }
        }



        protected AssetReference GetStoryPrefabAssetRefByStageNumber (int stageNumber) {
            int index = _currentStageNumber - 1;
            if (index >= 0 && index < _storyPrefabAssetRefs.Length) {
                return _storyPrefabAssetRefs[index];
            }
            return null;
        }


        protected virtual void OnStoryEnd (object sender, EventArgs args) {
            OnReadyToShowResult();
        }


        void OnStoryInstantiated (AsyncOperationHandle<GameObject> handle) {
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                GameObject storyObject = handle.Result;

                if (storyObject) {
                    StoryManager storyManager = storyObject.GetComponent<StoryManager>();
                    if (storyManager) {
                        storyManager.StoryEnded += OnStoryEnd;
                    }
                }
            }
        }

        

    }

}