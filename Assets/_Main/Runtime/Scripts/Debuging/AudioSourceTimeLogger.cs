using System;
using UnityEngine;

namespace ProjectComet.Debuging {

    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceTimeLogger : MonoBehaviour {
        
        AudioSource _audioSource;
        public AudioSource targetAudioSource => _audioSource ? _audioSource : _audioSource = this.GetComponent<AudioSource>();

        public void LogCurrentPlaybackTime () {
            if (targetAudioSource) {
                Debug.Log(targetAudioSource.time);
            }
        }

    }

}