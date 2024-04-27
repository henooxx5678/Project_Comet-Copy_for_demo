using System;
using UnityEngine;
using UnityEngine.Audio;
using DoubleHeat.Common;

namespace ProjectComet.OptionsHandling {


    public class AudioOptionsApplier : OptionsApplier {


        [SerializeField] AudioMixer _audioMixer;

        protected override void ApplyOptionsData(OptionsData data) {
            _audioMixer.SetFloat("Music Volume", GetDBVolume(data.MusicVolumeInFloat));
            _audioMixer.SetFloat("SFX Volume", GetDBVolume(data.SFXVolumeInFloat));
        }


        protected float GetDBVolume (float volumeRate) {
            
            return Mathf.Log(Mathf.Clamp(volumeRate, 0.001f, 1f)) * 20f;
        }

    }
}