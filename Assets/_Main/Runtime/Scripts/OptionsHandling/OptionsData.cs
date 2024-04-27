using System;
using UnityEngine;

namespace ProjectComet.OptionsHandling {

    [CreateAssetMenu(fileName = "OptionsData", menuName = "ScriptableObjects/OptionsData")]
    public class OptionsData : ScriptableObject {

        static float _baseNoteSpeed = 60f;


        public event Action AnyValueApplied;


        // Game Play
        [Range(0.5f, 5f)]
        [SerializeField] float _noteSpeedRate;
        public float NoteSpeedRate {
            get => _noteSpeedRate;
            set {
                _noteSpeedRate = Mathf.Clamp(value, 0.5f, 5f);
                AnyValueApplied?.Invoke();
            }
        }
        public float NoteSpeed {
            get => _baseNoteSpeed * NoteSpeedRate;
        }

        [Range(-0.255f, 0.255f)]
        [SerializeField] float _inputTimingCompensation;
        public float InputTimingCompensation {
            get => _inputTimingCompensation;
            set {
                _inputTimingCompensation = Mathf.Clamp(value, -0.255f, 0.255f);
                AnyValueApplied?.Invoke();
            }
        }

        // Audio
        [Range(0, 10)]
        [SerializeField] int _musicVolume;
        public int MusicVolume {
            get => _musicVolume;
            set {
                _musicVolume = value;
                _musicVolume = Math.Max(_musicVolume, 0);
                _musicVolume = Math.Min(_musicVolume, 10);
                AnyValueApplied?.Invoke();
            }
        }
        public float MusicVolumeInFloat => MusicVolume * 0.1f;

        [Range(0, 10)]
        [SerializeField] int _sfxVolume;
        public int SFXVolume {
            get => _sfxVolume;
            set {
                _sfxVolume = value;
                _sfxVolume = Math.Max(_sfxVolume, 0);
                _sfxVolume = Math.Min(_sfxVolume, 10);
                AnyValueApplied?.Invoke();
            }
        }
        public float SFXVolumeInFloat => SFXVolume * 0.1f;

        // Display
        protected bool? fullscreenNonApplied {get; set;} = null;

        protected Vector2Int? resolutionNonApplied {get; set;} = null;


        public OptionsData (OptionsData optionsData, bool resetDisplaySettings = false) {
            _noteSpeedRate = optionsData.NoteSpeedRate;
            _inputTimingCompensation = optionsData.InputTimingCompensation;
            _musicVolume = optionsData.MusicVolume;
            _sfxVolume = optionsData.SFXVolume;

            if (resetDisplaySettings) {
                ResetDisplaySettings();
            }
        }


        public void ResetDisplaySettings () {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
        }

        public bool GetFullscreenValue (bool resetToCurrent = false) {
            if (resetToCurrent || fullscreenNonApplied == null) {
                fullscreenNonApplied = Screen.fullScreen;
            }
            return (bool) fullscreenNonApplied;
        }

        public void SetTempFullscreenValue (bool fullscreen) {
            fullscreenNonApplied = fullscreen;
        }

        public Vector2Int GetResolutaionValue (bool resetToCurrent = false) {
            if (resetToCurrent || resolutionNonApplied == null) {
                resolutionNonApplied = new Vector2Int(Screen.width, Screen.height);
            }
            return (Vector2Int) resolutionNonApplied;
        }

        public void SetTempResolutionValue (Vector2Int resolution) {
            resolutionNonApplied = resolution;
        }


    }

}