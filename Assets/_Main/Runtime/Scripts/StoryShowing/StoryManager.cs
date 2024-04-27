using System;
using UnityEngine;
using ProjectComet.Common.UnityAnimations;

namespace ProjectComet.StoryShowing {

    public class StoryManager : MonoBehaviour {
        
        public event EventHandler<EventArgs> StoryEnded;


        [SerializeField] OutroEndAnimationEventTransmitter _outroEndAnimationEventTransmitter;


        protected virtual void OnEnable () {
            if (_outroEndAnimationEventTransmitter) {
                _outroEndAnimationEventTransmitter.OutroEnded += OnStoryEnd;
            }
        }

        protected virtual void OnDisable () {
            if (_outroEndAnimationEventTransmitter) {
                _outroEndAnimationEventTransmitter.OutroEnded -= OnStoryEnd;
            }
        }

        protected void OnStoryEnd () {
            StoryEnded?.Invoke(this, EventArgs.Empty);
        }

    }
}