using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using DG.Tweening;
using ProjectComet.CoreGameplay;

namespace ProjectComet.Calibration {
    

    public class MusicVolumeController : MonoBehaviour {
        
        [SerializeField] float _fadingDuration = 0f;

        [Header("REFS")]
        [SerializeField] AudioMixer _mixer;

        float _originalVolume = -1f;
        Tween _currentFadingTween = null;
        protected Tween currentFadingTween {
            get => _currentFadingTween;
            set {
                if (_currentFadingTween.IsActive()) {
                    _currentFadingTween.Kill(false);
                }
                _currentFadingTween = value;
            }
        }


        protected virtual void Awake () {
            InitOriginalVolume();
        }

        public void FadeOut () {
            try {
                InitOriginalVolume();

                float currentVol = _originalVolume;
                currentFadingTween = DOTween.To(
                    () => currentVol,
                    vol => {
                        currentVol = vol;
                        _mixer.SetFloat("Music Volume", RateToDB(vol));
                    },
                    0f,
                    _fadingDuration
                )
                    .SetUpdate(UpdateType.Normal, true);
            }
            catch (Exception e) {
                Debug.LogWarning(e);
            }
        }

        public void FadeIn () {
            try {
                float currentVolumeInDB;
                _mixer.GetFloat("Music Volume", out currentVolumeInDB);

                float currentVol = DBToRate(currentVolumeInDB);
                currentFadingTween = DOTween.To(
                    () => currentVol,
                    vol => {
                        currentVol = vol;
                        _mixer.SetFloat("Music Volume", RateToDB(vol));
                    },
                    _originalVolume,
                    _fadingDuration
                )
                    .SetUpdate(UpdateType.Normal, true);
            }
            catch (Exception e) {
                Debug.LogWarning(e);
            }
        }


        protected void InitOriginalVolume () {
            if (_originalVolume == -1f) {
                float currentVolumeInDB;
                _mixer.GetFloat("Music Volume", out currentVolumeInDB);

                _originalVolume = DBToRate(currentVolumeInDB);
            }
        }


        protected float DBToRate (float dB) {
            return Mathf.Pow(10f, dB / 20f);
        }

        protected float RateToDB (float rate) {
            return Mathf.Log(Mathf.Clamp(rate, 0.001f, 1f)) * 20f;
        }

    }

}
