using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ProjectComet.CoreGameplay;
using ProjectComet.CoreGameplay.Notes;
using ProjectComet.Levels;

namespace ProjectComet.Calibration {
    public class CalibrationLauncher : MonoBehaviour {

        public UnityEvent onLaunch;

        [SerializeField] TextAsset _midiJsonTextAsset;

        [Header("REFS")]        
        [SerializeField] Sheet _sheet;
        [SerializeField] AudioSource _audioSource;


        protected float audioClipDuration {
            get {
                if (_audioSource && _audioSource.clip) {
                    return _audioSource.clip.length;
                }
                return 0f;
            }
        }


        protected virtual void Start () {
            LoadNotes();
        }

        public void Launch () {
            LoadNotes();
            _sheet.StartPlayingWhenReady(0f, audioClipDuration, () => _audioSource.Play(), () => _audioSource.time);
            onLaunch?.Invoke();
        }

        
        protected void LoadNotes () {
            if (!_sheet.IsNotesLoaded) {
                Note[] notes = null;
                if (MidiDataReader.TryGetNotesFromMidiData(_midiJsonTextAsset.text, out notes)) {
                    _sheet.LoadNotes(notes);
                }
            }
        }

    }

}