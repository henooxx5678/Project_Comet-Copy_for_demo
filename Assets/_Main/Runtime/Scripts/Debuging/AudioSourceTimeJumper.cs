using System;
using UnityEngine;

namespace ProjectComet.Debuging {
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceTimeJumper : MonoBehaviour {
        
        AudioSource _audioSource;
        public AudioSource targetAudioSource => _audioSource ? _audioSource : _audioSource = this.GetComponent<AudioSource>();


        public void JumpTo (float time) {
            if (targetAudioSource) {
                targetAudioSource.time = Mathf.Max(time, 0f);
            }
        }

        public void JumpDeltaTime (float deltaTime) {
            if (targetAudioSource) {
                JumpTo(targetAudioSource.time + deltaTime);
            }
        }
    }

}