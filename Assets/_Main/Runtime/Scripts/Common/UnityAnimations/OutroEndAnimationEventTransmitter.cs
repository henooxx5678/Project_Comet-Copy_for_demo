using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityAnimations {

    public class OutroEndAnimationEventTransmitter : AnimationEventTransmitter {

        public event Action OutroEnded;

        public UnityEvent onOutroEnd;


        public void OutroEnd () {
            OutroEnded?.Invoke();
            onOutroEnd?.Invoke();
        }

    }

}