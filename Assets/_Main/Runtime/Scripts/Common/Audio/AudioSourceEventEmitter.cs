using UnityEngine;
using UnityEngine.Events;

namespace ProjectComet.Common.Audio {
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceEventEmitter : MonoBehaviour {

        AudioSource _targetAudioSource;
        protected AudioSource targetAudioSource {
            get {
                if (!_targetAudioSource) {
                    _targetAudioSource = this.GetComponent<AudioSource>();
                }
                return _targetAudioSource;
            }
        }


        public UnityEvent onPlay;
        public UnityEvent onStop;
        public UnityEvent onEndOfTrack;


        bool _isCurrentPlaying = false;


        protected virtual void Update () {

            bool isPlaying = targetAudioSource.isPlaying;

            if (isPlaying) {
                if (!_isCurrentPlaying) {
                    onPlay?.Invoke();
                }
            }
            else {
                if (_isCurrentPlaying) {
                    onStop?.Invoke();
                }

            }

            if (targetAudioSource.clip.length - targetAudioSource.time < Time.deltaTime) {
                onEndOfTrack?.Invoke();
            }

            _isCurrentPlaying = isPlaying;

        }
        
    }

}