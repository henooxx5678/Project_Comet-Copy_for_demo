using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DoubleHeat.Common;

namespace ProjectComet.OptionsHandling {

    public class OptionsAdjuster : MonoBehaviour {



        Vector2Int[] _availableResolutions = {
            new Vector2Int(640, 360),
            new Vector2Int(960, 540),
            new Vector2Int(1280, 720),
            new Vector2Int(1366, 768),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080)
        };
        protected Vector2Int[] availableResolutions => _availableResolutions;


        public UnityEvent onGamplayValueChanged;
        public UnityEvent onAudioValueChanged;
        public UnityEvent onVideoValueChanged;
        public UnityEvent onKeyboardValueChanged;


        protected OptionsHolder optionsHolder => OptionsHolder.current;
        protected OptionsData targetOptionsData => optionsHolder ? optionsHolder.CurrentOptions : null;


        void OnEnable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.ResetEvent += OnReset;
            }
        }


        void OnDisable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.ResetEvent -= OnReset;
            }
        }

        public void ApplyOptions () {
            optionsHolder.OnOptionsApplied();
        }

        // Gameplay
        public void SetDeltaNoteSpeedRate (float delta) {
            if (targetOptionsData) {
                SetNoteSpeedRate(targetOptionsData.NoteSpeedRate + delta);
            }
        }
        public void SetNoteSpeedRate (float value) {
            if (targetOptionsData) {
                targetOptionsData.NoteSpeedRate = value;

                onGamplayValueChanged?.Invoke();
            }
        }

        public void SetDeltaInputTimingCompensation (float delta) {
            if (targetOptionsData) {
                SetInputTimingCompensation(targetOptionsData.InputTimingCompensation + delta);
            }
        }
        public void SetInputTimingCompensation (float value) {
            if (targetOptionsData) {
                targetOptionsData.InputTimingCompensation = value;

                onGamplayValueChanged?.Invoke();
            }
        }

        // Audio
        public void SetDeltaMusicVolume (int delta) {
            if (targetOptionsData) {
                SetMusicVolume(targetOptionsData.MusicVolume + delta);
            }
        }
        public void SetMusicVolume (int value) {
            if (targetOptionsData) {
                targetOptionsData.MusicVolume = value;

                onAudioValueChanged?.Invoke();
            }
        }

        public void SetDeltaSFXVolume (int delta) {
            if (targetOptionsData) {
                SetSFXVolume(targetOptionsData.SFXVolume + delta);
            }
        }
        public void SetSFXVolume (int value) {
            if (targetOptionsData) {
                targetOptionsData.SFXVolume = value;

                onAudioValueChanged?.Invoke();
            }
        }

        // Video
        public void ToggleFullscreen () {
            if (targetOptionsData) {
                SetFullscreen(!targetOptionsData.GetFullscreenValue());
            }
        }
        public void SetFullscreen (bool fullscreen) {
            if (targetOptionsData) {
                targetOptionsData.SetTempFullscreenValue(fullscreen);

                onVideoValueChanged?.Invoke();
            }
        }

        public void SwitchToPrevAvailableResolutaion () {
            SwitchToAvailableResolutionOfIndex(GetCurrentIndexOfAvailableResolution() - 1);
        }
        public void SwitchToNextAvailableResolution () {
            SwitchToAvailableResolutionOfIndex(GetCurrentIndexOfAvailableResolution() + 1);
        }
        public void SetResolution (Vector2Int resolution) {
            if (targetOptionsData) {
                targetOptionsData.SetTempResolutionValue(resolution);

                onVideoValueChanged?.Invoke();
            }
        }
        protected void SwitchToAvailableResolutionOfIndex (int index) {
            index = Math.Max(index, 0);
            index = Math.Min(index, availableResolutions.Length - 1);

            SetResolution(availableResolutions[index]);
        }
        protected int GetCurrentIndexOfAvailableResolution () {
            if (targetOptionsData) {
                return Array.IndexOf(availableResolutions, targetOptionsData.GetResolutaionValue());
            }
            return -1;
        }


        // Keyboard
        public void StartListeningRebind (string actionPath) {
            if (OptionsHolder.current) {
                OptionsHolder.current.StartListeningLevelPlayingControlRebind(actionPath, () => {
                    onKeyboardValueChanged?.Invoke();
                });
            }
        }


        void OnReset () {
            onGamplayValueChanged?.Invoke();
            onVideoValueChanged?.Invoke();
            onAudioValueChanged?.Invoke();
            onKeyboardValueChanged?.Invoke();
        }

    }

}