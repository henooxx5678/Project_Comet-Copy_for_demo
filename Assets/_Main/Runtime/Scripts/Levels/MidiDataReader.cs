using System;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.Levels {

    public static class MidiDataReader {

        public static JSONObject GetJSONObjectFromJsonString (string jsonString) {
            return new JSONObject(jsonString);
        }

        public static bool TryGetBPMFromMidiData (string midiDataJson, out float bpm) {

            var midiJsonObject = GetJSONObjectFromJsonString(midiDataJson);
            return TryGetBPMFromMidiData(midiJsonObject, out bpm);
        }

        public static bool TryGetBPMFromMidiData (JSONObject midiJsonObject, out float bpm) {

            bpm = 0f;

            try {
                bpm = midiJsonObject["header"]["bpm"].floatValue;
            } 
            catch (Exception e) {
                Debug.Log("Fail to get BPM: " + e);
                return false;
            }

            return true;
        }

        public static bool TryGetNotesFromMidiData (string midiDataJson, out Note[] notes) {

            var midiJsonObject = GetJSONObjectFromJsonString(midiDataJson);
            return TryGetNotesFromMidiData(midiJsonObject, out notes);
        }

        public static bool TryGetNotesFromMidiData (JSONObject midiJsonObject, out Note[] notes) {
            
            notes = null;

            try {

                List<Note> noteList = new List<Note>();
                
                int trackIndex = 0;
                foreach (var track in midiJsonObject["tracks"].list) {

                    var notesInJson = track["notes"].list;
                    if (notesInJson != null && notesInJson.Count > 0) {

                        foreach (var noteInJson in notesInJson) {
                            Note note = GenerateNote(trackIndex, noteInJson["midi"].intValue, noteInJson["time"].floatValue, noteInJson["duration"].floatValue);
                            noteList.Add(note);
                        }
                        trackIndex++;
                    }
                }

                notes = noteList.ToArray();

            }
            catch (Exception e) {
                Debug.LogError(e);
                return false;
            }

            return true;
            
        }


        static Note GenerateNote (int trackIndex, int midiPitch, float time, float duration) {
            
            Note.Type noteType = Note.Type.None;
            
            switch (midiPitch) {
                case 60: {
                    noteType = Note.Type.Tap;
                    break;
                }
                case 62: {
                    noteType = Note.Type.Hold;
                    break;
                }
                case 64: {
                    noteType = Note.Type.TapCombination;
                    break;
                }
                case 65: {
                    noteType = Note.Type.Monster;
                    break;
                }
            }

            return new Note(noteType, trackIndex, time, time + duration);
        }


        // static void AccessData (JSONObject jsonObject) {
        //     switch (jsonObject.type) {
        //         case JSONObject.Type.Object:
        //             if (jsonObject.keys != null && jsonObject.list != null) {
        //                 for (var i = 0; i < jsonObject.list.Count; i++) {
        //                     var key = jsonObject.keys[i];
        //                     var value = jsonObject.list[i];
        //                     Debug.Log(key);
        //                     AccessData(value);
        //                 }
        //             }
        //             break;
        //         case JSONObject.Type.Array:
        //             // Debug.Log(jsonObject.list);
        //             if (jsonObject.list != null) {
        //                 foreach (JSONObject element in jsonObject.list) {
        //                     AccessData(element);
        //                 }
        //             }
        //             break;
        //         case JSONObject.Type.String:
        //             Debug.Log(jsonObject.stringValue);
        //             break;
        //         case JSONObject.Type.Number:
        //             Debug.Log(jsonObject.floatValue);
        //             break;
        //         case JSONObject.Type.Bool:
        //             Debug.Log(jsonObject.boolValue);
        //             break;
        //         case JSONObject.Type.Null:
        //             Debug.Log("Null");
        //             break;
        //         case JSONObject.Type.Baked:
        //             Debug.Log(jsonObject.stringValue);
        //             break;
        //     }
        // }

    }
}