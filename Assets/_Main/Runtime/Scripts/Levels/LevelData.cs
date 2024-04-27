using System;
using UnityEngine;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.Levels {

    public class LevelData {
        public int StageNumber {get; protected set;}
        public LevelDifficulty Difficulty {get; protected set;}
        public float Duration {get; protected set;}
        public float BPM {get; protected set;}
        public Note[] Notes {get; protected set;}
        public AudioClip SongAudioClip {get; protected set;}

        public LevelData (int stageNumber, LevelDifficulty difficulty, float duration, float bpm, Note[] notes, AudioClip songAudioClip) {
            StageNumber = stageNumber;
            Difficulty = difficulty;
            Duration = duration;
            BPM = bpm;
            Notes = notes;
            SongAudioClip = songAudioClip;
        }

        public override string ToString() {
            return $"StageNumber: {StageNumber}, Difficulty: {Difficulty}, Duration: {Duration}, BPM: {BPM}, NotesCount: {Notes.Length}";
        }

    }
}