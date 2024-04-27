using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay.Display {

    public class TracksDisplayController : MonoBehaviour, ITracksDisplayController {

        public bool IsExists => this;

        [SerializeField] int _tracksAmount = 3;
        public int TracksAmount => _tracksAmount;

        public float PreOnsetDuration => PreOnsetDistance / TravellingVelocity;
        public Transform ParentOfTracksDisplayElements => _tracksDisplayObject ? _tracksDisplayObject.transform : transform;
        public Transform NotesDisplayParent => _notesDisplayParent;


        [SerializeField] float _distanceBetweenTracks = 10f;
        public float DistanceBetweenTracks => _distanceBetweenTracks;
        public Vector3 RightStepVector => ParentOfTracksDisplayElements.TransformVector(Vector3.right * DistanceBetweenTracks);

        [SerializeField] float _preOnsetDistance = 1f;
        public float PreOnsetDistance {
            get => _preOnsetDistance;
            set => _preOnsetDistance = value;
        }

        [SerializeField] float _travellingVelocity = 1f;
        public float TravellingVelocity {
            get => _travellingVelocity;
            set => _travellingVelocity = value;
        }

        [Header("Debug Options")]
        [SerializeField] bool _showDebutLog;

        public Vector3 NotesLocalFlowingDirection => Vector3.back;

        public bool IsInited {get; protected set;} = false;


        [Header("REFS")]
        [SerializeField] GameObject _tracksDisplayObject; 
        [SerializeField] Transform _notesDisplayParent;

        [Header("Prefabs")]
        [SerializeField] NoteRelatedPrefabAssetInfo[] _noteDisplayPrefabAssetInfos;
        [SerializeField] NoteRelatedPrefabAssetInfo[] _noteHitVFXPrefabAssetInfos;
        [SerializeField] NoteRelatedPrefabAssetInfo[] _noteHoldedVFXPrefabAssetInfos;
        [SerializeField] NoteRelatedPrefabAssetInfo[] _noteReleasedVFXPrefabAssetInfos;
        [SerializeField] PercisionRankPrefabAssetInfo[] _percisionRankPrefabAssetInfos;


        Dictionary<Note.Type, GameObject> _noteDisplayPrefabs     = new Dictionary<Note.Type, GameObject>();
        Dictionary<Note.Type, GameObject> _noteHitVFXPrefabs      = new Dictionary<Note.Type, GameObject>();
        Dictionary<Note.Type, GameObject> _noteHoldedVFXPrefabs   = new Dictionary<Note.Type, GameObject>();
        Dictionary<Note.Type, GameObject> _noteReleasedVFXPrefabs = new Dictionary<Note.Type, GameObject>();
        Dictionary<NoteResult, GameObject> _percisionRankPrefabs = new Dictionary<NoteResult, GameObject>();
        

        // Level-running temp
        Dictionary<IInteractableNoteOnSheet, NoteDisplayInfo> _currentNotes = new Dictionary<IInteractableNoteOnSheet, NoteDisplayInfo>();

        // == MonoBehaviour Messages ==
        protected virtual void Awake () {
            LoadNotePrefabs(() => {
                IsInited = true;
            });
        }

        // == Public Methods ==
        public void OnPlayerStrike (int trackIndex) {

        }

        public void OnPlayerRelease (int trackIndex) {
            
        }

        public void ClearTracks () {
            _currentNotes.Clear();
            NotesDisplayParent.DestroyAllChildren();
        }

        public void NoteDebut (IInteractableNoteOnSheet noteOnSheet, float noteDurationInProgressDistance) {
            
            if (noteOnSheet.ExistedOnTracksIndex.Length > 0 && _noteDisplayPrefabs.ContainsKey(noteOnSheet.NoteType)) {

                int spawnedTrackIndex = noteOnSheet.ExistedOnTracksIndex[0];
                NoteDisplayController noteDisplayController = null;

                {
                    GameObject targetPrefab = _noteDisplayPrefabs[noteOnSheet.NoteType];
                    if (targetPrefab) {
                        GameObject noteDisplayObject = Instantiate(targetPrefab, NotesDisplayParent);

                        if (noteDisplayObject) {
                            noteDisplayController = noteDisplayObject.GetComponent<NoteDisplayController>();
                        }
                    }
                }

                if (noteDisplayController) {
                    noteDisplayController.transform.position = GetSpawnPositionOfTrack(spawnedTrackIndex);
                    noteDisplayController.Init(noteOnSheet, PreOnsetDistance * noteDurationInProgressDistance, NotesLocalFlowingDirection, RightStepVector);
                    
                    var noteDisplay = new NoteDisplayInfo(noteDisplayController.transform.position, noteDisplayController);

                    _currentNotes.Add(noteOnSheet, noteDisplay);

                    if (_showDebutLog) {
                        Debug.Log($"Note debut: {GetInfoOfInteractableNoteOnSheet(noteOnSheet)}");
                    }
                }
                else {
                    Debug.LogWarning("Failed to get \"NoteDisplayController\" of debuted note");
                }
            }
        }

        public void UpdateNotePosition (IInteractableNoteOnSheet noteOnSheet, float onsetProgress, float offsetProgress) {
            if (_currentNotes.ContainsKey(noteOnSheet)) {
                NoteDisplayInfo noteDisplayInfo = _currentNotes[noteOnSheet];

                if (noteDisplayInfo != null) {

                    noteDisplayInfo.CurrentOnsetProgress = onsetProgress;
                    noteDisplayInfo.CurrentOffsetProgress = offsetProgress;

                    SetNoteDisplayObjectPosition(_currentNotes[noteOnSheet], onsetProgress);

                    if (onsetProgress >= 1f) {
                        noteDisplayInfo.NoteDisplayController.OnOnsetReachedDestination();
                    }
                    if (offsetProgress >= 1f) {
                        noteDisplayInfo.NoteDisplayController.OnOffsetReachedDestination();
                    }

                }
            }
            else {
                // Debug.LogWarning($"Attempting to UPDATE the note which is not exist in the \"current notes\"! ({GetInfoOfInteractableNoteOnSheet(noteOnSheet)})");
            }
        }

        public void OnNoteHoldedThisFrame (IInteractableNoteOnSheet noteOnSheet) {
            if (_currentNotes.ContainsKey(noteOnSheet)) {
                NoteDisplayInfo noteDisplayInfo = _currentNotes[noteOnSheet];

                if (noteDisplayInfo != null) {
                    if (noteDisplayInfo.NoteDisplayController is TypeHoldNoteDisplayController) {
                        TypeHoldNoteDisplayController typeHoldNoteDisplayController = (TypeHoldNoteDisplayController) noteDisplayInfo.NoteDisplayController;

                        typeHoldNoteDisplayController.UpdateHoldedStatus(Mathf.Max(1f - noteDisplayInfo.CurrentOffsetProgress, 0f) * PreOnsetDistance);
                    }
                }
            }
            PlayNoteHoldedVFX(noteOnSheet);
        }

        public void DestroyNote (IInteractableNoteOnSheet noteOnSheet) {
            if (_currentNotes.ContainsKey(noteOnSheet)) {
                _currentNotes[noteOnSheet]?.OnDestroy();
                _currentNotes.Remove(noteOnSheet);
            }
            else {
                // Debug.LogError($"Attempting to DESTROY the note which is not exist in the \"current notes\"! ({GetInfoOfInteractableNoteOnSheet(noteOnSheet)})");
            }
        }

        public void OnNoteHit (IInteractableNoteOnSheet noteOnSheet, StrikeResult strikeResult = StrikeResult.None) {
            if (_currentNotes.ContainsKey(noteOnSheet)) {
                _currentNotes[noteOnSheet].NoteDisplayController.OnHit();
            }
            PlayNoteHitVFX(noteOnSheet);
        }

        public void OnNoteReleased (IInteractableNoteOnSheet noteOnSheet, StrikeResult strikeResult = StrikeResult.None) {
            if (strikeResult != StrikeResult.None) {
                if (_currentNotes.ContainsKey(noteOnSheet)) {
                    _currentNotes[noteOnSheet].NoteDisplayController.OnReleased();
                }
                PlayNoteReleasedVFX(noteOnSheet);
            }
            StopNoteHoldedVFX(noteOnSheet);
        }

        // public void OnStopHolding (IInteractableNoteOnSheet noteOnSheet) {
        //     StopNoteHoldedVFX(noteOnSheet);
        // }

        public void OnOverHolding (IInteractableNoteOnSheet noteOnSheet) {
            StopNoteHoldedVFX(noteOnSheet);
        }


        public void OnNoteResulted(IInteractableNoteOnSheet noteOnSheet, NoteResult noteResult) {
            PlayPercisionRankVFX(noteResult, noteOnSheet.ExistedOnTracksIndex);
        }


        public Vector3 GetSpawnPositionOfTrack (int trackIndex) {
            float horizontalPos = DistanceBetweenTracks * (trackIndex - (TracksAmount - 1) / 2f);
            return ParentOfTracksDisplayElements.TransformPoint(Vector3.forward * PreOnsetDistance + Vector3.right * horizontalPos);
        }

        public Vector3 GetTargetPositionOnTrack (int trackIndex) {
            return ParentOfTracksDisplayElements.transform.position + RightStepVector * (trackIndex - (TracksAmount - 1) / 2f);
        }


        // == Protected Methods ==
        protected void LoadNotePrefabs(Action endCallback = null) {

            var prefabLoadedCheckers = new List<Func<bool>>();

            LoadNoteRelatedPrefabsToDictionary(_noteDisplayPrefabAssetInfos, _noteDisplayPrefabs, prefabLoadedCheckers);
            LoadNoteRelatedPrefabsToDictionary(_noteHitVFXPrefabAssetInfos, _noteHitVFXPrefabs, prefabLoadedCheckers);
            LoadNoteRelatedPrefabsToDictionary(_noteHoldedVFXPrefabAssetInfos, _noteHoldedVFXPrefabs, prefabLoadedCheckers);
            LoadNoteRelatedPrefabsToDictionary(_noteReleasedVFXPrefabAssetInfos, _noteReleasedVFXPrefabs, prefabLoadedCheckers);
            LoadPercisionRankPrefabsToDictionary(_percisionRankPrefabAssetInfos, _percisionRankPrefabs, prefabLoadedCheckers);

            StartCoroutine(WaitingForCondition(prefabLoadedCheckers, endCallback));
        }

        void LoadPrefabsToDictionary (PrefabAssetInfo[] infos, Action<PrefabAssetInfo, GameObject> targetDictionaryAdder, Action<PrefabAssetInfo> loadedCheckerBuilder) {

            foreach (PrefabAssetInfo info in infos) {

                info.StartLoading(this, result => {
                    targetDictionaryAdder?.Invoke(info, result);
                    // targetDictionary.Add(info.NoteType, result);
                });

                loadedCheckerBuilder?.Invoke(info);
                // loadedCheckers.Add(() => targetDictionary.ContainsKey(info.NoteType));
            }
        }

        void LoadNoteRelatedPrefabsToDictionary (NoteRelatedPrefabAssetInfo[] infos, Dictionary<Note.Type, GameObject> targetDictionary, List<Func<bool>> loadedCheckers) {
            LoadPrefabsToDictionary(
                infos,
                (info, result) => targetDictionary.Add( ((NoteRelatedPrefabAssetInfo) info).NoteType, result ),
                info => loadedCheckers.Add(() => targetDictionary.ContainsKey( ((NoteRelatedPrefabAssetInfo)info).NoteType ))
            );
        }

        void LoadPercisionRankPrefabsToDictionary (PercisionRankPrefabAssetInfo[] infos, Dictionary<NoteResult, GameObject> targetDictionary, List<Func<bool>> loadedCheckers) {
            LoadPrefabsToDictionary(
                infos,
                (info, result) => targetDictionary.Add( ((PercisionRankPrefabAssetInfo) info).NoteResultType, result ),
                info => loadedCheckers.Add(() => targetDictionary.ContainsKey( ((PercisionRankPrefabAssetInfo) info).NoteResultType ))
            );
        }

        // == Private Methods ==
        IEnumerator WaitingForCondition(IEnumerable<Func<bool>> conditionGetters, Action endCallback) {
            while (true) {
                bool isEnded = true;
                foreach (Func<bool> conditionGetter in conditionGetters) {
                    if (conditionGetters == null || conditionGetter.Invoke() == false) {
                        isEnded = false;
                        break;
                    }
                }

                if (isEnded) {
                    break;
                }
                yield return null;
            }
            endCallback?.Invoke();
        }


        void SetNoteDisplayObjectPosition (NoteDisplayInfo noteDisplay, float noteProgress) {
            if (noteDisplay.NoteDisplayController) {
                noteDisplay.NoteDisplayController.transform.position = noteDisplay.DebutOriginPosition + ParentOfTracksDisplayElements.TransformVector(Vector3.back * PreOnsetDistance * noteProgress);
            }
        }

        string GetInfoOfInteractableNoteOnSheet (IInteractableNoteOnSheet noteOnSheet) {
            return $"(type {noteOnSheet.NoteType}, track {noteOnSheet.ExistedOnTracksIndex[0]})";
        }

        void PlayNoteHitVFX (IInteractableNoteOnSheet noteOnSeet) {
            Note.Type noteType = noteOnSeet.NoteType;
            foreach (int trackIndex in noteOnSeet.ExistedOnTracksIndex) {
                if (_currentNotes.ContainsKey(noteOnSeet) && _currentNotes[noteOnSeet] != null) {
                    if (_noteHitVFXPrefabs.ContainsKey(noteType) && _noteHitVFXPrefabs[noteType]) {
                        _currentNotes[noteOnSeet].HitVFXInstance = Instantiate(_noteHitVFXPrefabs[noteType], GetTargetPositionOnTrack(trackIndex), Quaternion.identity, NotesDisplayParent);
                    }
                }
            }
        }

        void PlayNoteHoldedVFX (IInteractableNoteOnSheet noteOnSheet) {
            Note.Type noteType = noteOnSheet.NoteType;
            foreach (int trackIndex in noteOnSheet.ExistedOnTracksIndex) {
                if (_currentNotes.ContainsKey(noteOnSheet) && _currentNotes[noteOnSheet] != null) {
                    if (_noteHoldedVFXPrefabs.ContainsKey(noteType) && _noteHoldedVFXPrefabs[noteType]) {
                        if (!_currentNotes[noteOnSheet].HoldedVFXInstance) {
                            _currentNotes[noteOnSheet].HoldedVFXInstance = Instantiate(_noteHoldedVFXPrefabs[noteType], GetTargetPositionOnTrack(trackIndex), Quaternion.identity, NotesDisplayParent);
                        }
                    }
                }
            }
        }

        void StopNoteHoldedVFX (IInteractableNoteOnSheet noteOnSheet) {
            if (_currentNotes.ContainsKey(noteOnSheet) && _currentNotes[noteOnSheet] != null) {
                if (_currentNotes[noteOnSheet].HoldedVFXInstance) {
                    Destroy(_currentNotes[noteOnSheet].HoldedVFXInstance);
                }
            }
        }

        void PlayNoteReleasedVFX (IInteractableNoteOnSheet noteOnSheet) {
            Note.Type noteType = noteOnSheet.NoteType;
            foreach (int trackIndex in noteOnSheet.ExistedOnTracksIndex) {
                if (_currentNotes.ContainsKey(noteOnSheet) && _currentNotes[noteOnSheet] != null) {
                    if (_noteReleasedVFXPrefabs.ContainsKey(noteType) && _noteReleasedVFXPrefabs[noteType]) {
                        _currentNotes[noteOnSheet].ReleasedVFXInstance = Instantiate(_noteReleasedVFXPrefabs[noteType], GetTargetPositionOnTrack(trackIndex), Quaternion.identity, NotesDisplayParent);
                    }
                }
            }
        }

        void PlayPercisionRankVFX (NoteResult noteResult, int trackIndex) {
            if (_percisionRankPrefabs.ContainsKey(noteResult) && _percisionRankPrefabs[noteResult] != null) {
                Instantiate(_percisionRankPrefabs[noteResult], GetTargetPositionOnTrack(trackIndex), Quaternion.identity, ParentOfTracksDisplayElements);
            }
        }

        void PlayPercisionRankVFX (NoteResult noteResult, int[] tracksIndex) {
            if (tracksIndex != null) {
                foreach (int trackIndex in tracksIndex) {
                    PlayPercisionRankVFX(noteResult, trackIndex);
                }
            }
        }


        // == Nested Classes/Structs ==
        class NoteDisplayInfo {
            public Vector3 DebutOriginPosition {get; protected set;}
            public NoteDisplayController NoteDisplayController {get; protected set;}
            
            public float CurrentOnsetProgress {get; set;} = Mathf.NegativeInfinity;
            public float CurrentOffsetProgress {get; set;} = Mathf.NegativeInfinity;

            public GameObject HitVFXInstance {get; set;} = null;
            public GameObject HoldedVFXInstance {get; set;} = null;
            public GameObject ReleasedVFXInstance {get; set;} = null;

            public NoteDisplayInfo (Vector3 debutOriginPosition, NoteDisplayController noteDisplayController) {
                DebutOriginPosition = debutOriginPosition;
                NoteDisplayController = noteDisplayController;
            }

            public void OnDestroy () {
                if (NoteDisplayController && NoteDisplayController.gameObject) {
                    Destroy(NoteDisplayController.gameObject);
                }

                if (HoldedVFXInstance) {
                    try {
                        Destroy(HoldedVFXInstance);
                    }
                    finally {
                        Debug.LogWarning("[bug] A \"HoldedVFXInstance\" which shouldn't remained has been removed!");
                    }
                }
            }
        }

        [Serializable]
        protected class PrefabAssetInfo {
            public AssetReference PrefabAssetRef;

            public void StartLoading (MonoBehaviour coroutineCarrier, Action<GameObject> loadedCallback) {
                AsyncOperationHandle<GameObject> prefabLoading = default;

                if (PrefabAssetRef != null) {
                    try {
                        prefabLoading = PrefabAssetRef.LoadAssetAsync<GameObject>();
                    }
                    catch (InvalidKeyException e) {
                        Debug.LogError("Failed to load note prefab: " + e);
                    }

                }

                coroutineCarrier.StartCoroutine(LoadingPrefab(prefabLoading, loadedCallback));
            }

            IEnumerator LoadingPrefab (AsyncOperationHandle<GameObject> loadingOperationHandle, Action<GameObject> loadedCallBack) {
                if (loadingOperationHandle.IsValid()) {
                    yield return loadingOperationHandle;
                    loadedCallBack?.Invoke(loadingOperationHandle.Result);
                }
                else {
                    loadedCallBack?.Invoke(null);
                }
            }
        }

        [Serializable]
        protected class NoteRelatedPrefabAssetInfo : PrefabAssetInfo {
            public Note.Type NoteType;
        }

        [Serializable]
        protected class PercisionRankPrefabAssetInfo : PrefabAssetInfo {
            public NoteResult NoteResultType;            
            
        }

    }
}
