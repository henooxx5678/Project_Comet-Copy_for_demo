using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectComet.CoreGameplay;
using ProjectComet.CoreGameplay.Notes;
using DoubleHeat.Utilities;

namespace ProjectComet.Levels {

    public class LevelLauncher : MonoBehaviour {

        public event EventHandler<EventArgs> LevelLaunched;

        public bool HasLaunched => _levelData != null;


        [Header("Level Starting")]
        [SerializeField] float _preStartDelay = 0f;

        [Header("Options")]
        [SerializeField] bool _willLoadMidiDataFromStreamingAssets;

        [Header("REFS")]
        [SerializeField] LevelPlayingStatesManager _levelPlayingStatesManager;
        [SerializeField] AudioSource _levelSongAudioSource;
        [SerializeField] Sheet _sheet;
        [SerializeField] InLevelScoringController _inLevelScoringController;
        [SerializeField] LevelResultHandler _levelResultHandler;
        
        [SerializeField] MonoBehaviour[] _initableComponents;
        IInitableWithLevel[] _initables;
        protected IInitableWithLevel[] initables {
            get {
                if (_initables == null) {
                    _initables = new IInitableWithLevel[_initableComponents.Length];
                    for (int i = 0 ; i < _initables.Length ; i++) {
                        _initables[i] = MonoBehaviourTools.GetConvertedMonoBehaviour<IInitableWithLevel>(_initableComponents[i]);
                    }
                }
                return _initables;
            }
        }


        LevelData _levelData = null;


        public void StartLaunching (int stageNumber, LevelDifficulty difficulty) {
            if (HasLaunched) {
                StopPlayingLevel();
            }
            StartCoroutine(Launching(stageNumber, difficulty));
        }


        public void RestartLevel () {
            if (HasLaunched) {

                StopPlayingLevel();
                StartLevel(_levelData);
    
            }
        }
        
        protected IEnumerator Launching (int stageNumber, LevelDifficulty difficulty) {
            LevelDataLoader levelDataLoader = new LevelDataLoader(stageNumber, difficulty, _willLoadMidiDataFromStreamingAssets);

            yield return StartCoroutine(levelDataLoader.Loading());

            _levelData = levelDataLoader.Result;

            if (_levelPlayingStatesManager) {
                _levelPlayingStatesManager.RestartLevelAction = RestartLevel;
            }
            else {
                Debug.LogWarning("\"_levelPlayingStatesManager\" is missing! Restart function will be unavailable.");
            }
            
            if (_sheet) {
                _sheet.LoadNotes(_levelData.Notes, _levelData.BPM);
            }
            else {
                Debug.LogWarning("\"_sheet\" is missing!");
            }

            if (_levelSongAudioSource) {
                _levelSongAudioSource.clip = _levelData.SongAudioClip;
            }
            else {
                Debug.LogWarning("\"_levelSongAudioSource\" is missing!");
            }


            StartLevel(_levelData);
            Debug.Log("Level Launched: " + _levelData);

            LevelLaunched?.Invoke(this, new LevelLaunchedEventArgs{
                stageNumber = stageNumber,
                difficulty = difficulty
            });
        }

        void StartLevel (LevelData levelData) {

            if (_levelResultHandler) {
                _levelResultHandler.Init(BestRecordsManager.GetBestRecord(levelData.StageNumber, levelData.Difficulty));
            }
            else {
                Debug.LogWarning("\"_levelResultHandler\" is missing!");
            }

            _levelPlayingStatesManager.OnLevelStart();
            _inLevelScoringController.Reset(levelData.Difficulty);
            _sheet.StartPlayingWhenReady(_preStartDelay, levelData.Duration, PlayLevelSong, () => _levelSongAudioSource.time);

            foreach (IInitableWithLevel initable in initables) {
                initable.InitWithLevel(levelData.StageNumber, levelData.Difficulty);
            }
        }

        void StopPlayingLevel () {
            _sheet.StopPlaying();
            _levelSongAudioSource.Stop();
        }

        void PlayLevelSong () {
            _levelSongAudioSource?.Play();
            Debug.Log("Song played.");
        }


        public class LevelLaunchedEventArgs : EventArgs {
            public int stageNumber;
            public LevelDifficulty difficulty;
        }
    }
}