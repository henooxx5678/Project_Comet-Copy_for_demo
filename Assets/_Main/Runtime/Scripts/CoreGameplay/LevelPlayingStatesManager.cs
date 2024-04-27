using System;
using System.Collections;
using UnityEngine;

namespace ProjectComet.CoreGameplay {
    
    public class LevelPlayingStatesManager : MonoBehaviour {

        public event EventHandler<EventArgs> LevelStarted;
        public event EventHandler<EventArgs> LevelEnded;


        public Action RestartLevelAction {get; set;}
        public Action ExitLevelAction {get; set;}
        public Action ExitLevelAndNextAction {get; set;}


        [SerializeField] GameObject _pausePopUpObject;




        PlayingStatus _currentPausingStatus;
        protected PlayingStatus currentPausingStatus {
            get {
                if (_currentPausingStatus == null) {
                    _currentPausingStatus = new PlayingStatus(
                        this,
                        () => {
                            Time.timeScale = 0f;
                            if (_levelSongAudioSource.isPlaying) {
                                _levelSongAudioSource.Pause();
                            }
                        },
                        () => {
                            Time.timeScale = 1f;
                            if (!_levelSongAudioSource.isPlaying && _levelSongAudioSource.time > 0f) {
                                _levelSongAudioSource.UnPause();
                            }
                        }
                    );
                }
                return _currentPausingStatus;
            }
        }
        public IPlayingStatus CurrentPausingStatus => currentPausingStatus;


        [SerializeField] float _resumingCountDownDuration = 3f;

        [Header("REFS")]
        [SerializeField] Sheet _sheet;
        [SerializeField] AudioSource _levelSongAudioSource;

        [SerializeField] Component _unpauseCountDownDisplayControllerComponent;
        IUnpauseCountDownDisplayController _unpauseCountDownDisplayController;
        protected IUnpauseCountDownDisplayController unpauseCountDownDisplayController {
            get {
                if (_unpauseCountDownDisplayController == null && _unpauseCountDownDisplayControllerComponent is MonoBehaviour) {
                    _unpauseCountDownDisplayController = (IUnpauseCountDownDisplayController) _unpauseCountDownDisplayControllerComponent;
                }
                return _unpauseCountDownDisplayController;
            }
        }

        
        protected virtual void OnEnable () {
            if (_sheet) {
                _sheet.RoundStarted += OnSheetStartPlaying;
                _sheet.RoundEnded += OnSheetPlayedToEnd;
            }
            else {
                Debug.LogWarning("\"_sheet\" is empty!");
            }
        }

        protected virtual void OnDisable () {
            if (_sheet) {
                _sheet.RoundStarted -= OnSheetStartPlaying;
                _sheet.RoundEnded -= OnSheetPlayedToEnd;
            }
        }


        public void OnLevelStart () {
            currentPausingStatus.OnLevelStart();
        }

        public void Restart () {
            // TODO: outro anim
            Resume(false);
            RestartLevelAction?.Invoke();
        }

        public void Exit () {
            ExitLevel(false);
        }

        public void ExitAndNext () {
            ExitLevel(true);
        }



        public void Pause () {
            currentPausingStatus.Pause();

            if (_pausePopUpObject) {
                _pausePopUpObject.SetActive(true);
            }
        }


        public void Resume () {
            Resume(true);
        }

        public void Resume (bool countdown) {
            float countdownDuration = countdown ? _resumingCountDownDuration : 0f;

            currentPausingStatus.Resume(
                countdownDuration,
                remainTime => {
                    if (unpauseCountDownDisplayController != null) {
                        unpauseCountDownDisplayController.UpdateDisplay((int) remainTime + 1);

                        if (remainTime > 0) {
                            unpauseCountDownDisplayController.Show();
                        }
                        else {
                            unpauseCountDownDisplayController.Hide();
                        }   
                    }
                }
            );

            if (_pausePopUpObject) {
                _pausePopUpObject.SetActive(false);
            }
        }

        protected void ExitLevel (bool goNext) {
            // TODO: outro anim            
            Time.timeScale = 1f;
            
            if (goNext) {
                ExitLevelAndNextAction?.Invoke();
            }
            else {
                ExitLevelAction?.Invoke();
            }
        }




        protected virtual void OnSheetStartPlaying (object sender, EventArgs args) {
            LevelStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSheetPlayedToEnd (object sender, EventArgs args) {
            currentPausingStatus.End();
            LevelEnded?.Invoke(this, EventArgs.Empty);
        }


        // == Nested Classes/Structs/Interfaces ==
        public interface IPlayingStatus {
            event Action<PlayingState> CurrentPlayingStateChanged;

            Action OnPauseCallback { get; }
            Action OnUnpauseCallback { get; }

            PlayingState CurrentPlayingState { get; }
            float CurrentRemainingTimeWhenCountingDownForResume { get; }
        }

        protected class PlayingStatus : IPlayingStatus {
            public event Action<PlayingState> CurrentPlayingStateChanged;

            public Action OnPauseCallback { get; set; } = null;
            public Action OnUnpauseCallback { get; set; } = null;

            PlayingState _currentPlayingState = PlayingState.Playing;
            public PlayingState CurrentPlayingState {
                get => _currentPlayingState;
                set {
                    _currentPlayingState = value;
                    CurrentPlayingStateChanged?.Invoke(_currentPlayingState);
                }
            }
            public float CurrentRemainingTimeWhenCountingDownForResume {get; private set;} = 0f;

            MonoBehaviour _coroutineCarrier;
            Coroutine _currentCountingDownToUnpauseCoroutine;
            

            public PlayingStatus (MonoBehaviour coroutineCarrier, Action onPauseCallback, Action onUnpauseCallback) {
                _coroutineCarrier = coroutineCarrier;
                OnPauseCallback = onPauseCallback;
                OnUnpauseCallback = onUnpauseCallback;
            }

            public void OnLevelStart () {
                CurrentPlayingState = PlayingState.Playing;
            }

            public void Pause () {
                if (CurrentPlayingState != PlayingState.Paused) {
                    CurrentPlayingState = PlayingState.Paused;
                    
                    OnPauseCallback?.Invoke();

                    if (_currentCountingDownToUnpauseCoroutine != null) {
                        _coroutineCarrier.StopCoroutine(_currentCountingDownToUnpauseCoroutine);
                    }

                    Debug.Log("Paused");
                }
            }

            public void Resume (float countdownDuration, Action<float> countDownTimeSetter = null) {
                if (CurrentPlayingState == PlayingState.Paused) {
                    if (_coroutineCarrier) {
                        _currentCountingDownToUnpauseCoroutine = _coroutineCarrier.StartCoroutine(CountingDownToUnpause(countdownDuration, countDownTimeSetter));
                        CurrentPlayingState = PlayingState.CountingDownForResume;
                    }
                    else {
                        Debug.LogError("\"_coroutineCarrier\" does not exist!");
                        Unpause();
                    }
                }
            }

            public void End () {
                CurrentPlayingState = PlayingState.Ended;
                Debug.Log("Ended");
            }

            IEnumerator CountingDownToUnpause (float countdownDuration, Action<float> countDownTimeSetter = null) {
                float startTime = Time.realtimeSinceStartup;
                float elapsedTime = 0f;

                while (true) {
                    elapsedTime = Time.realtimeSinceStartup - startTime;

                    float remainedTime = countdownDuration - elapsedTime;
                    CurrentRemainingTimeWhenCountingDownForResume = Mathf.Max(remainedTime, 0f);

                    countDownTimeSetter?.Invoke(CurrentRemainingTimeWhenCountingDownForResume);

                    if (remainedTime > 0) {
                        yield return null;
                    }
                    else {
                        break;
                    }
                }

                Unpause();
            }

            void Unpause () {

                OnUnpauseCallback?.Invoke();

                CurrentPlayingState = PlayingState.Playing;
                Debug.Log("Resumed");
            }

            
        }


        public enum PlayingState {
            Playing,
            Paused,
            CountingDownForResume,
            Ended
        }

    }
}