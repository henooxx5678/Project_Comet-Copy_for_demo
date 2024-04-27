using System;
using System.Collections;
using UnityEngine;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay.Display {

    [DisallowMultipleComponent]
    public class NoteDisplayController : MonoBehaviour {

        public bool HasBeenHit {get; protected set;} = false;
        public bool HasBeenReleased {get; protected set;} = false;

        protected bool hasOnsetReachedDestination {get; set;} = false;
        protected bool hasOffsetReachedDestination {get; set;} = false;


        protected Vector3 _flowingDirection {get; private set;} = Vector3.zero;


        public virtual void Init (IInteractableNoteOnSheet noteOnSheet, float distanceOfNoteDuration, Vector3 flowingLocalDirection, Vector3 rightStepVectorOfTracks) {
            _flowingDirection = flowingLocalDirection;
        }

        public virtual void OnHit () {
            HasBeenHit = true;
            UpdateStatus();
        }

        public virtual void OnReleased () { 
            HasBeenReleased = true;
            UpdateStatus();
        }

        public virtual void OnOnsetReachedDestination () {
            if (hasOnsetReachedDestination)
                return;

            hasOnsetReachedDestination = true;
            UpdateStatus();
        }

        public virtual void OnOffsetReachedDestination () {
            if (hasOffsetReachedDestination)
                return;

            hasOffsetReachedDestination = true;
            UpdateStatus();
        }


        protected virtual void UpdateStatus () {

        }

    }

}
