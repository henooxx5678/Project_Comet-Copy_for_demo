using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.Levels {

    public class LevelDataLoader {

        const float _LEVEL_RELEASE_DURATION_AFTER_LAST_NOTE_OFFSET = 3f;

        const string _LEVEL_MIDI_DATA_DIR_NAME_OF_ADDRESSABLE_ASSETS = "Assets/_Main/Runtime/JSON Files/MIDI Data";
        const string _LEVEL_SONG_DATA_DIR_NAME = "Assets/_Audios/Level Songs";
        readonly string _LEVEL_MIDI_DATA_DIR_NAME_OF_STREAMING_ASSETS = Application.streamingAssetsPath + "/Level Data";


        public event EventHandler<EventArgs> LoadingFailed;
        public event EventHandler<EventArgs> LoadingCompleted;


        public bool IsLoading {get; private set;} = false;
        public LevelData Result {get; private set;} = null;



        int _stageNumber = -1;
        LevelDifficulty _difficulty = LevelDifficulty.None;
        bool _willLoadMidiDataFromStreamingAssets = false;


        public LevelDataLoader (int stageNumber, LevelDifficulty difficulty, bool willLoadMidiDataFromStreamingAssets = false) {
            _stageNumber = stageNumber;
            _difficulty = difficulty;
            _willLoadMidiDataFromStreamingAssets = willLoadMidiDataFromStreamingAssets;
        }

        public IEnumerator Loading () {
            IsLoading = true;
            Result = null;

            string difficultyLabel = string.Empty;
            switch (_difficulty) {
                case LevelDifficulty.Normal: {
                    difficultyLabel = "easy";
                    break;
                }
                case LevelDifficulty.Hard: {
                    difficultyLabel = "hard";
                    break;
                }
            }

            if (difficultyLabel != string.Empty) {

                float bpm = 0f;
                Note[] notes = null;
                AudioClip songAudioClip = null;

                string midiDataJson = string.Empty;                


                // Load BPM & notes
                if (!_willLoadMidiDataFromStreamingAssets) {

                    string midiDataJsonAddress = $"{_LEVEL_MIDI_DATA_DIR_NAME_OF_ADDRESSABLE_ASSETS}/stage_{_stageNumber}_{difficultyLabel}.json";
                    var midiDataJsonAssetLoading = new AsyncOperationHandle<TextAsset>();

                    try {
                        midiDataJsonAssetLoading = Addressables.LoadAssetAsync<TextAsset>(midiDataJsonAddress);
                    }
                    catch (Exception e) {
                        Debug.LogWarning("Invalid MIDI data json address. Error message: " + e);
                    }

                    yield return midiDataJsonAssetLoading;

                    midiDataJson = midiDataJsonAssetLoading.Result.text;

                }
                else {
                    
                    string midiDataJsonAddress = $"{_LEVEL_MIDI_DATA_DIR_NAME_OF_STREAMING_ASSETS}/stage_{_stageNumber}_{difficultyLabel}.json";

                    if (File.Exists(midiDataJsonAddress)) {
                        midiDataJson = File.ReadAllText(midiDataJsonAddress);
                    }
                    else {
                        Debug.LogWarning("Cannot find target MIDI data file.");
                    }

                }

                var midiJsonObject = MidiDataReader.GetJSONObjectFromJsonString(midiDataJson);

                if (!MidiDataReader.TryGetBPMFromMidiData(midiJsonObject, out bpm)) {
                    Debug.LogWarning("Failed to read BPM from midi data.");
                }

                if (!MidiDataReader.TryGetNotesFromMidiData(midiJsonObject, out notes)) {
                    Debug.LogWarning("Failed to read NOTES from midi data.");
                }
                


                // Load song
                string songAudioAssetAddress = $"{_LEVEL_SONG_DATA_DIR_NAME}/stage_{_stageNumber}.wav";
                var songAudioAssetLoading = new AsyncOperationHandle<AudioClip>();

                try {
                    songAudioAssetLoading = Addressables.LoadAssetAsync<AudioClip>(songAudioAssetAddress);
                }
                catch (Exception e) {
                    Debug.LogWarning("Invalid audio data address. Error message: " + e);
                }

                yield return songAudioAssetLoading;

                songAudioClip = songAudioAssetLoading.Result;


                // Generate result LevelData
                float duration = Mathf.Max(Note.GetMaxOffsetOfNotes(notes) + _LEVEL_RELEASE_DURATION_AFTER_LAST_NOTE_OFFSET, songAudioClip ? songAudioClip.length : 0f);
                Result = new LevelData(_stageNumber, _difficulty, duration, bpm, notes, songAudioClip);

            }

            IsLoading = false;

            if (Result == null) {
                LoadingFailed?.Invoke(this, EventArgs.Empty);
            }
            else {
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
            }

        }


        

    }
}