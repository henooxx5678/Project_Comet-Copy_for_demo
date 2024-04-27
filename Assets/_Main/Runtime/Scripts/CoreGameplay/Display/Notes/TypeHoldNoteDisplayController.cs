using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay.Notes;
using ProjectComet.CoreGameplay.Display.Curtains;

namespace ProjectComet.CoreGameplay.Display {

    public class TypeHoldNoteDisplayController : NoteDisplayController {

        const float _PIXELS_PER_METER = 100f;

        [SerializeField] GameObject _headNoteObject;
        [SerializeField] GameObject _tailNoteObject;
        [SerializeField] Transform _linkerDisplayStartPoint;
        [SerializeField] GameObject _linkerDisplayObject;



        public override void Init (IInteractableNoteOnSheet noteOnSheet, float distanceOfNoteDuration, Vector3 flowingLocalDirection, Vector3 rightStepVectorOfTracks) {
            base.Init(noteOnSheet, distanceOfNoteDuration, flowingLocalDirection, rightStepVectorOfTracks);

            if (_headNoteObject) {
                _headNoteObject.transform.localPosition = Vector3.zero;
            }
            if (_tailNoteObject) {
                _tailNoteObject.transform.localPosition = _headNoteObject.transform.localPosition - flowingLocalDirection * distanceOfNoteDuration;
            }
            // if (_linkerDisplayObject) {
            //     UpdateLinkerDisplay(_headNoteObject.transform.position, distanceOfNoteDuration, flowingLocalDirection);
            // }
            
        }


        protected virtual void LateUpdate () {
            if (_tailNoteObject && _linkerDisplayStartPoint) {
                UpdateLinkerDisplay(_linkerDisplayStartPoint.position, Vector3.Dot(_linkerDisplayStartPoint.position - _tailNoteObject.transform.position, _flowingDirection), _flowingDirection);
            }
        }


        public override void OnReleased () {
            base.OnReleased();
        }

        public void UpdateHoldedStatus (float distanceFromDestinationToOffset) {
            float distance = Mathf.Max(distanceFromDestinationToOffset, 0f);

            if (_tailNoteObject && _linkerDisplayStartPoint) {
                _linkerDisplayStartPoint.position = _tailNoteObject.transform.position + _flowingDirection * distance;
                // UpdateLinkerDisplay(_tailNoteObject.transform.position + _flowingDirection * distance, distance, _flowingDirection);
            }

            // if (distanceFromDestinationToOffset <= 0) {
            //     if (this.gameObject) {
            //         this.gameObject.SetActive(false);
            //     }
            // }
        }
        
        protected override void UpdateStatus () {
            base.UpdateStatus();

            if (HasBeenHit && hasOnsetReachedDestination) {
                if (_headNoteObject) {
                    _headNoteObject.SetActive(false);
                }
            }

            if (HasBeenReleased) {
                if (this.gameObject) {
                    this.gameObject.SetActive(false);
                }
            }
        }


        void UpdateLinkerDisplay (Vector3 startPos, float distanceOfDuration, Vector3 flowingDirection) {
            float distance = Mathf.Infinity;

            foreach (Curtain curtain in Curtain.Instances) {
                distance = Mathf.Clamp(distance, 0f, Vector3.Dot(startPos - curtain.transform.position, curtain.transform.forward));
            }
            distance = Mathf.Clamp(distance, 0f, distanceOfDuration);


            _linkerDisplayObject.transform.position = startPos - flowingDirection * distance / 2f;

            float zOfParentLossyScale = 1f;
            {
                Transform parent = _linkerDisplayObject.transform.parent;
                if (parent) {
                    zOfParentLossyScale = (_linkerDisplayObject.transform.rotation * parent.lossyScale).z;
                    if (zOfParentLossyScale == 0) {
                        zOfParentLossyScale = 1f;
                    }
                }
            }

            _linkerDisplayObject.transform.localScale = _linkerDisplayObject.transform.localScale.GetAfterSetZ(distance / zOfParentLossyScale);
        }

        // obsoleted for SpriteRenderer
        // void SetLinkerDisplayDistance (float distance) {

        //     if (_linkerDisplayObject) {
        //         float parentLossyScaleY = 1f;

        //         Transform parent = _linkerDisplayObject.transform.parent;
        //         if (parent) {
        //             parentLossyScaleY = Vector3.Dot(parent.lossyScale, _linkerDisplayObject.transform.up);
        //             if (parentLossyScaleY <= 0) {
        //                 parentLossyScaleY = 1f;
        //             }
        //         }
                
        //         _linkerDisplayObject.transform.localScale = _linkerDisplayObject.transform.localScale.GetAfterSetY(distance * _PIXELS_PER_METER / parentLossyScaleY);
        //     }
        // }
    }

}