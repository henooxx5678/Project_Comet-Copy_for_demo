using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DoubleHeat.Common;

namespace ProjectComet.OptionsHandling {

    
    public class OptionsHolder : SingletonMonoBehaviour<OptionsHolder> {
        
        public static readonly float InputTimingCompensationStep = 0.017f;

        static class ParamNames {
            public const string NOTE_SPEED_RATE = "NoteSpeedRate";
            public const string INPUT_TIMING_COMPENSATION = "InputTimingCompensation";
            public const string MUSIC_VOLUME = "MusicVolume";
            public const string SFX_VOLUME = "SFXVolume";
            public const string GAME_PLAY_CONTROL_REBINDS = "GamePlayControlRebinds";
        }


        public event Action ResetEvent;
        public event EventHandler<EventArgs> OptionsApplied;
        public event Action<string> RebindingStatusUpdated;
        public event Action DuplicatedRebindPerformed;


        [SerializeField] InputActionAsset _levelPlayingActions;
        [SerializeField] OptionsData _defaultOptionsData;
        


        bool _isIniting = false;


        OptionsData _currentOptions;
        public OptionsData CurrentOptions {
            get {
                if (!_currentOptions) {
                    _currentOptions = new OptionsData(_defaultOptionsData, true);
                    _currentOptions.AnyValueApplied += OnOptionsDataAnyValueApplied;
                }
                return _currentOptions;
            }
        }


        protected virtual void OnEnable () {
            LoadOptionsFromPlayerPref();
        }


        public void ResetBindingToDefault () {
            PlayerPrefs.DeleteKey(ParamNames.GAME_PLAY_CONTROL_REBINDS);

            _levelPlayingActions.RemoveAllBindingOverrides();
            OnRebindingComplete();
            
            ResetEvent?.Invoke();
        }

        public void OnOptionsApplied () {
            OptionsApplied?.Invoke(this, new OptionsDataEventArgs{ optionsData = CurrentOptions });
            SaveCurrentOptionsToPlayerPref();
        }


        // Level Playing Control Rebinds
        public void RemoveLevelPlayingControlRebinds () {
            _levelPlayingActions.RemoveAllBindingOverrides();
        }

        public void StartListeningLevelPlayingControlRebind (string actionPath, Action completedCallback = null) {
            if (_levelPlayingActions) {
                InputAction action = _levelPlayingActions.FindAction(actionPath);
                if (action != null) {

                    var rebindingOperation = action.PerformInteractiveRebinding();
                    
                    rebindingOperation.OnPotentialMatch( operation => {

                        bool duplicated = false;

                        if (operation.candidates.Count > 0) {
                            InputControl candidateInputControl = operation.candidates[0];

                            InputActionMap playingActionMap = _levelPlayingActions.FindActionMap("Playing");
                            if (playingActionMap != null) {
                                foreach (InputAction inputAction in playingActionMap.actions) {

                                    if (Array.Exists(inputAction.controls.ToArray(), control => control == candidateInputControl)) {
                                        duplicated = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (duplicated) {
                            DuplicatedRebindPerformed?.Invoke();

                            RebindingStatusUpdated?.Invoke(null);
                            operation.Dispose();

                            Debug.Log("Duplicated rebinding performed.");
                        }
                    } );

                    rebindingOperation.OnComplete( operation => {
                        completedCallback?.Invoke();

                        RebindingStatusUpdated?.Invoke(null);
                        operation.Dispose();

                        OnRebindingComplete();

                        Debug.Log("Rebinding complete.");
                    } );

                    rebindingOperation.Start();
                    RebindingStatusUpdated?.Invoke(actionPath);
                }
                else {
                    Debug.LogWarning("Action not found.");
                }
            }
            else {
                Debug.LogWarning("\"_levelPlayingAction\" is empty!");
            }
        }


        public void SaveCurrentOptionsToPlayerPref () {
            PlayerPrefs.SetFloat(ParamNames.NOTE_SPEED_RATE, CurrentOptions.NoteSpeedRate);
            PlayerPrefs.SetFloat(ParamNames.INPUT_TIMING_COMPENSATION, CurrentOptions.InputTimingCompensation);
            PlayerPrefs.SetInt(ParamNames.MUSIC_VOLUME, CurrentOptions.MusicVolume);
            PlayerPrefs.SetInt(ParamNames.SFX_VOLUME, CurrentOptions.SFXVolume);
            PlayerPrefs.SetString(ParamNames.GAME_PLAY_CONTROL_REBINDS, _levelPlayingActions.SaveBindingOverridesAsJson());
        }

        public void LoadOptionsFromPlayerPref () {
            _isIniting = true;

            if (PlayerPrefs.HasKey(ParamNames.NOTE_SPEED_RATE)) {
                CurrentOptions.NoteSpeedRate = PlayerPrefs.GetFloat(ParamNames.NOTE_SPEED_RATE, CurrentOptions.NoteSpeedRate);
            }
            if (PlayerPrefs.HasKey(ParamNames.INPUT_TIMING_COMPENSATION)) {
                CurrentOptions.InputTimingCompensation = PlayerPrefs.GetFloat(ParamNames.INPUT_TIMING_COMPENSATION, CurrentOptions.InputTimingCompensation);
            }
            if (PlayerPrefs.HasKey(ParamNames.MUSIC_VOLUME)) {
                CurrentOptions.MusicVolume = PlayerPrefs.GetInt(ParamNames.MUSIC_VOLUME, CurrentOptions.MusicVolume);
            }
            if (PlayerPrefs.HasKey(ParamNames.SFX_VOLUME)) {
                CurrentOptions.SFXVolume = PlayerPrefs.GetInt(ParamNames.SFX_VOLUME, CurrentOptions.SFXVolume);
            }
            if (PlayerPrefs.HasKey(ParamNames.GAME_PLAY_CONTROL_REBINDS)) {
                string rebindsContent = PlayerPrefs.GetString(ParamNames.GAME_PLAY_CONTROL_REBINDS);
                if (!string.IsNullOrEmpty(rebindsContent)) {
                    _levelPlayingActions.LoadBindingOverridesFromJson(rebindsContent);
                }
            }

            OnOptionsApplied();

            _isIniting = false;
        }


        protected void OnOptionsDataAnyValueApplied () {
            if (!_isIniting) {
                OnOptionsApplied();
            }
        }

        protected void OnRebindingComplete () {
            SaveCurrentOptionsToPlayerPref();
        }


        public class OptionsDataEventArgs : EventArgs {
            public OptionsData optionsData;
        }

    }
}