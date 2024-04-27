using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay.Display {

    public class TypeTapCombinationNoteDisplayController : NoteDisplayController {
        
        [SerializeField] GameObject _firstNoteObject;
        [SerializeField] GameObject _linkerDisplayObject;

        List<GameObject> _spawnedNoteObjects = new List<GameObject>();

        public override void Init (IInteractableNoteOnSheet noteOnSheet, float distanceOfNoteDuration, Vector3 flowingLocalDirection, Vector3 rightStepVectorOfTracks) {
            base.Init(noteOnSheet, distanceOfNoteDuration, flowingLocalDirection, rightStepVectorOfTracks);

            int[] existedTrackIndex = noteOnSheet.ExistedOnTracksIndex;

            if (existedTrackIndex.Length > 0) {

                int firstTrackIndex = existedTrackIndex[0];

                for (int i = 1 ; i < existedTrackIndex.Length ; i++) {
                    int stepsFromFirst = existedTrackIndex[i] - firstTrackIndex;

                    if (_firstNoteObject) {
                        GameObject noteObject = Instantiate(_firstNoteObject, _firstNoteObject.transform.position + rightStepVectorOfTracks * stepsFromFirst, Quaternion.identity, _firstNoteObject.transform.parent);
                        _spawnedNoteObjects.Add(noteObject);
                    }
                }
                
                // Setup linker display
                int maxSteps = existedTrackIndex[existedTrackIndex.Length - 1] - existedTrackIndex[0];
                Vector3 extents = rightStepVectorOfTracks * (maxSteps);
                _linkerDisplayObject.transform.position = _firstNoteObject.transform.position + extents / 2f;

                Vector3 parentLossyScale = _linkerDisplayObject.transform.parent ? _linkerDisplayObject.transform.parent.lossyScale : Vector3.one;
                float xOfParentLossyScale = (_linkerDisplayObject.transform.rotation * parentLossyScale).x;
                if (xOfParentLossyScale != 0) {
                    _linkerDisplayObject.transform.localScale = _linkerDisplayObject.transform.localScale.GetAfterSetX( Vector3.Dot(extents, _linkerDisplayObject.transform.right) / xOfParentLossyScale );
                }

            }
            else {
                this.gameObject.SetActive(false);
            }
        }


        protected override void UpdateStatus() {
            base.UpdateStatus();
            if (HasBeenHit && hasOnsetReachedDestination) {
                Destroy(gameObject);
            }
        }
    }

}