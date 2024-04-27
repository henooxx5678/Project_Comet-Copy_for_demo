using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay {

    public class SheetEventHandler : MonoBehaviour {

        [Header("REFS")]
        [SerializeField] Sheet _sheet;

        [Header("Events")]
        [SerializeField] UnityEvent<int> _onPlayerStrike;
        [SerializeField] UnityEvent<int> _onPlayerRelease;
        [SerializeField] UnityEvent<int> _onPerfect;
        [SerializeField] UnityEvent<int> _onGood;
        [SerializeField] UnityEvent<int> _onMiss;
        [SerializeField] UnityEvent<int> _onSafe;
        [SerializeField] UnityEvent<int> _onFail;

        protected virtual void OnEnable () {
            if (_sheet) {
                _sheet.PlayerStruck += OnPlayerStrike;
                _sheet.PlayerReleased += OnPlayerRelease;
                _sheet.NoteStruck += OnNoteStruck;
                _sheet.NoteMissed += OnNoteMissed;
                _sheet.TypeHoldNoteFailedToBeCompleted += OnTypeHoldNoteFailedToBeCompleted;
                _sheet.MonsterPassed += OnMonsterPassed;
            }
            else {
                Debug.LogWarning("\"_sheet\" is empty!");
            }
        }

        protected virtual void OnDisable () {
            if (_sheet) {
                _sheet.PlayerStruck -= OnPlayerStrike;
                _sheet.PlayerReleased -= OnPlayerRelease;
                _sheet.NoteStruck -= OnNoteStruck;
                _sheet.NoteMissed -= OnNoteMissed;
                _sheet.TypeHoldNoteFailedToBeCompleted -= OnTypeHoldNoteFailedToBeCompleted;
                _sheet.MonsterPassed -= OnMonsterPassed;
            }
        }


        protected virtual void OnPlayerStrike (object sender, EventArgs args) {
            if (args is Sheet.TrackEventArgs) {
                Sheet.TrackEventArgs trackEventArgs = (Sheet.TrackEventArgs) args;

                _onPlayerStrike?.Invoke(trackEventArgs.trackIndex);
            }
        }

        protected virtual void OnPlayerRelease (object sender, EventArgs args) {
            if (args is Sheet.TrackEventArgs) {
                Sheet.TrackEventArgs trackEventArgs = (Sheet.TrackEventArgs) args;
                
                _onPlayerRelease?.Invoke(trackEventArgs.trackIndex);
            }
        }

        protected virtual void OnNoteStruck (object sender, EventArgs args) {
            if (args is Sheet.NoteStruckEventArgs) {
                Sheet.NoteStruckEventArgs noteStruckEventArgs = (Sheet.NoteStruckEventArgs) args;

                UnityEvent<int> eventToBeEmitted = null;

                switch (noteStruckEventArgs.strikeResult) {
                    case StrikeResult.Perfect: {
                        eventToBeEmitted = _onPerfect;
                        break;
                    }
                    case StrikeResult.Good: {
                        eventToBeEmitted = _onGood;
                        break;
                    }
                }

                EmitEventTracks(eventToBeEmitted, GetTrackIndexFromArgs(args));
            }
        }

        protected virtual void OnNoteMissed (object sender, EventArgs args) {
            EmitEventTracks(_onMiss, GetTrackIndexFromArgs(args));
        }

        protected virtual void OnTypeHoldNoteFailedToBeCompleted(object sender, EventArgs args) {
            EmitEventTracks(_onMiss, GetTrackIndexFromArgs(args));
        }

        protected virtual void OnMonsterPassed(object sender, EventArgs args) {
            if (args is Sheet.MonsterPassedEventArgs) {
                Sheet.MonsterPassedEventArgs monsterPassedEventArgs = (Sheet.MonsterPassedEventArgs) args;
                
                UnityEvent<int> eventToBeEmitted = null;

                switch (monsterPassedEventArgs.monsterPassResult) {
                    case MonsterPassResult.Safe: {
                        eventToBeEmitted = _onSafe;
                        break;
                    }
                    case MonsterPassResult.Fail: {
                        eventToBeEmitted = _onFail;
                        break;
                    }
                }

                EmitEventTracks(eventToBeEmitted, GetTrackIndexFromArgs(args));
            }
        }


        protected void EmitEventTracks (UnityEvent<int> eventToBeEmitted, int[] tracksIndex) {
            foreach (int trackIndex in tracksIndex) {
                eventToBeEmitted?.Invoke(trackIndex);
            }
        }


        int[] GetTrackIndexFromArgs (EventArgs args) {
            if (args is Sheet.InteractableNoteResultedOnTrackEventArgs) {
                Sheet.InteractableNoteResultedOnTrackEventArgs interactableNoteResultedOnTrackEventArgs = (Sheet.InteractableNoteResultedOnTrackEventArgs) args;
                if (interactableNoteResultedOnTrackEventArgs != null && interactableNoteResultedOnTrackEventArgs.targetInteractableNote != null) {
                    return interactableNoteResultedOnTrackEventArgs.targetInteractableNote.ExistedOnTracksIndex ?? new int[0];
                }
            }
            return new int[0];
        }

    }
}