using System;
using UnityEngine;
using DG.Tweening;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay {

    public interface ITracksDisplayController {

        bool      IsExists                      { get; }
        int       TracksAmount                  { get; }
        float     PreOnsetDuration              { get; }
        Transform ParentOfTracksDisplayElements { get; }
        float     DistanceBetweenTracks         { get; }
        Vector3   RightStepVector               { get; }
        bool      IsInited                      { get; }

        void OnPlayerStrike  (int trackIndex);
        void OnPlayerRelease (int trackIndex);
        void ClearTracks ();
        void NoteDebut             (IInteractableNoteOnSheet note, float noteDurationInProgressDistance);
        void UpdateNotePosition    (IInteractableNoteOnSheet note, float onsetProgress, float offsetProgress) ;
        void OnNoteHoldedThisFrame (IInteractableNoteOnSheet note);
        void DestroyNote           (IInteractableNoteOnSheet note);

        void OnNoteHit             (IInteractableNoteOnSheet note, StrikeResult strikeResult);
        void OnNoteReleased        (IInteractableNoteOnSheet note, StrikeResult strikeResult);
        // void OnStopHolding         (IInteractableNoteOnSheet note);
        void OnOverHolding         (IInteractableNoteOnSheet note);

        void OnNoteResulted        (IInteractableNoteOnSheet note, NoteResult noteResult);
    }

}