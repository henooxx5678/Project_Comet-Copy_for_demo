using System;
using System.Collections;
using UnityEngine;
using ProjectComet.CoreGameplay.ScoringStatus;
using ProjectComet.Levels;

namespace ProjectComet.CoreGameplay {

    public class InLevelScoringController : MonoBehaviour {

        [SerializeField] Sheet _sheet;
        [SerializeField] ScoringStatusManager _scoringStatusManager;


        protected virtual void OnEnable () {
            if (_sheet) {
                _sheet.NoteStruck += OnNoteStruck;
                _sheet.NoteMissed += OnNoteMissed;
                _sheet.TypeHoldNoteFailedToBeCompleted += OnNoteMissed;
                _sheet.MonsterPassed += OnMonsterPassed;
            }
        }

        protected virtual void OnDisable () {
            if (_sheet) {
                _sheet.NoteStruck -= OnNoteStruck;
                _sheet.NoteMissed -= OnNoteMissed;
                _sheet.TypeHoldNoteFailedToBeCompleted -= OnNoteMissed;
                _sheet.MonsterPassed -= OnMonsterPassed;
            }
        }
        

        public void Reset (LevelDifficulty difficulty) {
            if (_sheet.IsNotesLoaded) {
                _scoringStatusManager.Init(_sheet.ScorableNotesCount, difficulty);
            }
            else {
                Debug.LogWarning("Reset failed!");
            }
        }


        protected virtual void OnNoteStruck (object sender, EventArgs args) {

            if (args is Sheet.NoteStruckEventArgs) {
                Sheet.NoteStruckEventArgs noteStruckEventArgs = (Sheet.NoteStruckEventArgs) args;

                if (_scoringStatusManager) {
                    _scoringStatusManager.ApplyAffect( ScoringStatusManager.ConvertToResultTypeFromStrikeResult(noteStruckEventArgs.strikeResult) );
                }
            }
        }

        protected virtual void OnNoteMissed (object sender, EventArgs args) {

            if (args is Sheet.InteractableNoteResultedOnTrackEventArgs) {
                Sheet.InteractableNoteResultedOnTrackEventArgs noteResultedEventArgs = (Sheet.InteractableNoteResultedOnTrackEventArgs)args;

                if (_scoringStatusManager) {
                    _scoringStatusManager.ApplyAffect(ScoringStatusManager.ResultType.Miss);
                }
            }
        }

        protected virtual void OnMonsterPassed (object sender, EventArgs args) {
            if (args is Sheet.MonsterPassedEventArgs) {
                Sheet.MonsterPassedEventArgs monsterPassedEventArgs = (Sheet.MonsterPassedEventArgs) args;

                if (_scoringStatusManager) {
                    _scoringStatusManager.ApplyAffect( ScoringStatusManager.ConvertToResultTypeFromMonsterPassResult(monsterPassedEventArgs.monsterPassResult) );
                }
            }

        }
        
    }
}