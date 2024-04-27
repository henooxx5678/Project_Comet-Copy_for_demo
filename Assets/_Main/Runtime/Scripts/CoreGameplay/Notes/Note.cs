using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectComet.CoreGameplay.Notes {

    public class Note : IOffsetTimeCarrier {

        public static Note Empty = new Note();


        public Type NoteType {get; protected set;}
        public int TrackIndex {get; protected set;}

        public float Onset {get; protected set;}
        public float Offset {get; protected set;}


        public bool IsEmpty => NoteType == Type.None && TrackIndex < 0;
        public float Duration => Offset - Onset;


        public static Queue<Note> GenerateNotesQueue (IEnumerable<Note> notes) {
            if (notes != null) {
                return new Queue<Note>(notes.OrderBy(note => note.Onset).ThenBy(note => note.TrackIndex));
            }
            return null;
        }

        public static float GetMaxOffsetOfNotes (IEnumerable<Note> notes) {
            if (notes != null) {
                return notes.Max(note => note.Offset);
            }
            return 0f;
        }


        public Note () {
            NoteType = Type.None;
            TrackIndex = -1;
            Onset = -1f;
            Offset = -1f;
        }

        public Note (Type type, int trackIndex, float onset, float offset = -1f) {
            NoteType = type;
            TrackIndex = trackIndex;
            Onset = onset;
            Offset = type == Type.Hold && offset != -1f ? offset : onset;
        }



        public override string ToString () {
            return $"[Note] - type: {NoteType}, track: {TrackIndex}, onset: {Onset}, offset: {Offset}, duration: {Duration}";
        }


        public enum Type {
            None,
            Tap,
            Hold,
            TapCombination,
            Monster
        }

    }
}
