using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoubleHeat.Utilities;
using ProjectComet.CoreGameplay.Notes;

namespace ProjectComet.CoreGameplay {

    public class Sheet : MonoBehaviour {

        public event EventHandler<EventArgs> RoundStarted;
        public event EventHandler<EventArgs> RoundEnded;

        public event EventHandler<EventArgs> PlayerStruck;
        public event EventHandler<EventArgs> PlayerReleased;

        public event EventHandler<EventArgs> StrikeMissed;
        public event EventHandler<EventArgs> NoteStruck;
        public event EventHandler<EventArgs> NoteMissed;
        public event EventHandler<EventArgs> TypeHoldNoteFailedToBeCompleted;
        public event EventHandler<EventArgs> MonsterPassed;


        #region Fields & Properties
        public bool IsUsingRealTime => _isUsingRealTime;
        public float TypeHoldNoteSegmentDuration {get; protected set;} = Mathf.Infinity;


        [SerializeField] bool _isUsingRealTime = false;
        [SerializeField] float _minTypeHoldNoteGapBetweenSegmentEndPointAndMinNoteTailPoint = 0.005f;
        public float MinTypeHoldNoteGapBetweenSegmentEndPointAndMinNoteTailPoint => _minTypeHoldNoteGapBetweenSegmentEndPointAndMinNoteTailPoint;

        [SerializeField] int _trackCount = 3;
        public int TrackCount {
            get => _trackCount;
            set => _trackCount = value;
        }

        [SerializeField] float _sheetTimeShift = 0f;

        [SerializeField] float _inputCompensationInSeconds;
        public float InputCompensationInSeconds {
            get => _inputCompensationInSeconds;
            set => _inputCompensationInSeconds = value;
        }


        [SerializeField] StrikeTolerance[] _strikeTolerances;
        StrikeTolerance[] _sortedStrikeTolerances;
        protected StrikeTolerance[] sortedStrikeTolerances {
            get {
                _sortedStrikeTolerances ??= StrikeTolerance.GetSortedRangeLargeToSmall(_strikeTolerances);
                return _sortedStrikeTolerances;
            }
        }

        [SerializeField] float _monsterTolerancesInSeconds = 0.12f;
        [SerializeField] float _strikeCooldownWhenMissed = 0.12f;
        [SerializeField] float _delayedDurationToDestroy = 0.5f;


        [Header("REFS")]
        [SerializeField] Component _tracksDisplayControllerComponentType;
        ITracksDisplayController _tracksDisplayController = null;
        public ITracksDisplayController TracksDisplayController {
            get {
                if (_tracksDisplayController == null && _tracksDisplayControllerComponentType is ITracksDisplayController) {
                    _tracksDisplayController = (ITracksDisplayController) _tracksDisplayControllerComponentType;
                }
                return _tracksDisplayController;
            }
        }

        [Header("Debug Options")]
        [SerializeField] bool _showMissedNoteLog;
        [SerializeField] bool _showPlayerMissedStrikeLog;


        public float PreOnsetDuration {
            get {
                if (TracksDisplayController != null) {
                    return TracksDisplayController.PreOnsetDuration;
                }
                // Debug.LogWarning("TracksDisplayController is null!");
                return 0f;
            }
        }


        StrikeResult _maxToleranceStrikeResult = StrikeResult.None;
        public StrikeResult MaxToleranceStrikeResult {
            get {
                if (_maxToleranceStrikeResult == StrikeResult.None && sortedStrikeTolerances != null) {
                    _maxToleranceStrikeResult = StrikeTolerance.GetMaxToleranceStrikeResult(sortedStrikeTolerances);
                }
                return _maxToleranceStrikeResult;
            }
        }

        float _maxStrikeToleranceInSeconds = -1f;
        public float MaxStrikeToleranceInSeconds {
            get {
                if (_maxStrikeToleranceInSeconds < 0 && sortedStrikeTolerances != null) {
                    _maxStrikeToleranceInSeconds = StrikeTolerance.GetMaxToleranceInSeconds(sortedStrikeTolerances);
                }
                return _maxStrikeToleranceInSeconds;
            }
        }

        public bool IsNotesLoaded => _notesQueue != null;
        public bool IsReady => IsNotesLoaded && isInited;
        public bool IsPlaying {get; protected set;} = false;
        public int NotesOnSheetCount {
            get {
                if (IsNotesLoaded) {
                    var notesOnSheet = GetInteractableNotesOnSheetQueue();
                    if (notesOnSheet != null) {
                        return notesOnSheet.Count;
                    }
                }
                return 0;
            }
        }
        public int ScorableNotesCount {
            get {
                if (IsNotesLoaded) {
                    int sum = 0;
                    var notesOnSheet = GetInteractableNotesOnSheetQueue();
                    foreach (InteractableNoteOnSheet interactableNote in notesOnSheet) {
                        sum += interactableNote.ScorableNoteCount;
                    }
                    return sum;
                }
                return 0;
            }
        }

        protected bool isInited { get; private set; } = false;


        Queue<Note> _notesQueue;

        Coroutine _currentPlayingCoroutine = null;
        protected Coroutine currentPlayingCoroutine {
            get {
                return _currentPlayingCoroutine;
            }
            set {
                if (_currentPlayingCoroutine != null) {
                    StopCoroutine(_currentPlayingCoroutine);
                }
                _currentPlayingCoroutine = value;
            }
        }


        PlayerStrikesChecker _playerStrikesChecker = null;
        int _currentPlayerPositionNumber = -1;


        #if UNITY_EDITOR
            [Header("Debug Functions")]
            [SerializeField] bool loop = false;
        #endif
        #endregion


        #region Methods
        protected virtual void OnEnable () {
            _playerStrikesChecker = new PlayerStrikesChecker(TrackCount, _strikeCooldownWhenMissed);
            _playerStrikesChecker.PlayerStruck += (sender, args) => PlayerStruck?.Invoke(this, args);
            _playerStrikesChecker.PlayerReleased += (sender, args) => PlayerReleased?.Invoke(this, args);
            isInited = true;
        }

        protected virtual void OnDisable () {
            _playerStrikesChecker = null;
            currentPlayingCoroutine = null;
            isInited = false;
        }



        public void LoadNotes (IEnumerable<Note> notes, float bpm = 0f) {
            LoadNotes(Note.GenerateNotesQueue(notes), bpm);
        }

        public void LoadNotes (Queue<Note> notesQueue, float bpm = 0f) {
            _notesQueue = notesQueue ?? new Queue<Note>();

            if (bpm > 0) {
                TypeHoldNoteSegmentDuration = 60f / bpm;
            }
            else {
                TypeHoldNoteSegmentDuration = Mathf.Infinity;
            }
        }


        public void StartPlayingWhenReady (float preStartDelay, float duration, Action songStartPlayingCallback, Func<float> currentPlayingTimeGetter) {
            StartCoroutine(GoingToStartPlayingWhenReady(preStartDelay, duration, songStartPlayingCallback, currentPlayingTimeGetter));
        }

        public void StopPlaying () {
            StopAllCoroutines();
            TracksDisplayController?.ClearTracks();
        }


        public void OnPlayerStrike (int trackIndex) {
            _playerStrikesChecker.AddStrikeThisFrame(trackIndex);
            TracksDisplayController?.OnPlayerStrike(trackIndex);
        }

        public void OnPlayerRelease (int trackIndex) {
            _playerStrikesChecker.AddReleaseThisFrame(trackIndex);
        }

        public void UpdatePlayerPosition (int positionNumber) {
            _currentPlayerPositionNumber = positionNumber;
        }



        public static int GetWholeSegmentCountOfTypeHoldNote (float noteDuration, float segmentDuration, float minGapBetweenSegmentEndPointAndMinNoteTailPoint, float minOfNoteTailRange) {
            return (int) ((noteDuration + minOfNoteTailRange - minGapBetweenSegmentEndPointAndMinNoteTailPoint) / segmentDuration);
        }

        protected static Queue<InteractableNoteOnSheet> GetInteractableNotesOnSheetQueueFromNotesQueue (Queue<Note> sourceNotesQueue, float typeHoldNoteSegmentDuration, float minGapBetweenSegmentEndPointAndMinNoteTailPoint, float typeHoldNoteMinOfNoteTailRange, ITracksDisplayController tracksDisplayController = null) {
            var notesQueue = new Queue<Note>(sourceNotesQueue);
            var result = new Queue<InteractableNoteOnSheet>();

            List<Note> notesOfTypeTapCombinationPerStep = new List<Note>();

            void AddCurrentTypeTapCombinationStepToResult () {
                if (notesOfTypeTapCombinationPerStep.Count > 0) {

                    Note firstNote = notesOfTypeTapCombinationPerStep[0];

                    int[] tracksIndex = new int[notesOfTypeTapCombinationPerStep.Count];
                    for (int i = 0; i < tracksIndex.Length; i++) {
                        tracksIndex[i] = notesOfTypeTapCombinationPerStep[i].TrackIndex;
                    }

                    result.Enqueue(new InteractableTypeTapCombinationNoteOnSheet(firstNote.NoteType, firstNote.Onset, tracksIndex));
                    notesOfTypeTapCombinationPerStep.Clear();
                }
            }


            Note prevNoteForDuplicatedNoteChecking = Note.Empty;

            while (notesQueue.Count > 0) {

                Note note = notesQueue.Dequeue();
                Note firstNoteOfPrevStep = notesOfTypeTapCombinationPerStep.Count > 0 ? notesOfTypeTapCombinationPerStep[0] : null;

                // Ignore duplicated
                {
                    Note prevNote = prevNoteForDuplicatedNoteChecking;
                    prevNoteForDuplicatedNoteChecking = note;
                    
                    if (!prevNote.IsEmpty) {
                        if (prevNote.Onset == note.Onset && prevNote.TrackIndex == note.TrackIndex) {
                            if ((prevNote.NoteType != Note.Type.Monster) == (note.NoteType != Note.Type.Monster)) {
                                Debug.LogWarning("A duplicated note has been ignored by the sheet: " + note);
                                continue;
                            }
                        }
                    }
                }


                bool isTypeTapCombinationStepCompleted = false;

                if (firstNoteOfPrevStep != null && firstNoteOfPrevStep.Onset == note.Onset) {

                    if (note.NoteType == Note.Type.TapCombination && firstNoteOfPrevStep.NoteType == Note.Type.TapCombination) {
                        notesOfTypeTapCombinationPerStep.Add(note);
                        continue;
                    }
                }
                else {
                    isTypeTapCombinationStepCompleted = true;
                }

                if (isTypeTapCombinationStepCompleted) {
                    AddCurrentTypeTapCombinationStepToResult();
                }

                if (note.NoteType == Note.Type.TapCombination) {
                    notesOfTypeTapCombinationPerStep.Add(note);
                }
                else {
                    switch (note.NoteType) {
                        case Note.Type.Tap: {
                            result.Enqueue(new InteractableTypeTapNoteOnSheet(note.NoteType, note.Onset, note.TrackIndex));
                            break;
                        }
                        case Note.Type.Hold: {
                            result.Enqueue(new InteractableTypeHoldNoteOnSheet(note.NoteType, note.Onset, note.Offset, note.TrackIndex, GetWholeSegmentCountOfTypeHoldNote(note.Duration, typeHoldNoteSegmentDuration, minGapBetweenSegmentEndPointAndMinNoteTailPoint, typeHoldNoteMinOfNoteTailRange)));
                            break;
                        }
                        case Note.Type.Monster: {
                            result.Enqueue(new MonsterNoteOnSheet(note.NoteType, note.Onset, note.TrackIndex));
                            break;
                        }
                    }

                }
            }

            AddCurrentTypeTapCombinationStepToResult();


            if (tracksDisplayController != null) {
                foreach (InteractableNoteOnSheet noteOnSheet in result) {
                    if (noteOnSheet is StrikableNoteOnSheet) {

                        StrikableNoteOnSheet strikableNoteOnSheet = (StrikableNoteOnSheet) noteOnSheet;

                        strikableNoteOnSheet.OnHitCallback = strikeResult => tracksDisplayController.OnNoteHit(strikableNoteOnSheet, strikeResult);
                        strikableNoteOnSheet.OnReleasedCallback = strikeResult => tracksDisplayController.OnNoteReleased(strikableNoteOnSheet, strikeResult);
                        // strikableNoteOnSheet.OnStopHoldingCallback = () => tracksDisplayController.OnStopHolding(strikableNoteOnSheet);
                        strikableNoteOnSheet.OnOverHoldingCallback = () => tracksDisplayController.OnOverHolding(strikableNoteOnSheet);

                    }
                }
            }

            return result;
        }


        protected Queue<InteractableNoteOnSheet> GetInteractableNotesOnSheetQueue (ITracksDisplayController trackDisplayController = null) {
            return GetInteractableNotesOnSheetQueueFromNotesQueue(_notesQueue, TypeHoldNoteSegmentDuration, MinTypeHoldNoteGapBetweenSegmentEndPointAndMinNoteTailPoint, -StrikeTolerance.GetMaxToleranceInSeconds(_strikeTolerances), trackDisplayController);
        }

        protected void StartPlaying (float duration, Action songStartPlayingCallback, Func<float> currentPlayingTimeGetter) {
            currentPlayingCoroutine = StartCoroutine(Playing(duration, songStartPlayingCallback, currentPlayingTimeGetter));
        }
          
        protected float GetTargetTimePositionTravellingProgress (float targetTimePos, float elapsedTime) {
            return (elapsedTime - GetTargetTimePositionDebutTimeFromPlayingStart(targetTimePos)) / PreOnsetDuration;
        }

        protected float GetTargetTimePositionDebutTimeFromPlayingStart (float targetTimePos) {
            return targetTimePos + _sheetTimeShift - PreOnsetDuration;
        }

        protected float GetProgressDistanceByDuration (float duration) {
            return duration / PreOnsetDuration;
        }


        protected StrikeResult GetStrikeResult(float relativeToTargetTime) {
            StrikeResult strikeResult = StrikeResult.None;
            foreach (StrikeTolerance strikeTolerance in sortedStrikeTolerances) {
                if (Mathf.Abs(relativeToTargetTime) < strikeTolerance.ToleranceInSeconds) {
                    strikeResult = strikeTolerance.StrikeResult;
                } 
                else {
                    break;
                }
            }
            return strikeResult;
        }

        protected float GetCurrentTime () {
            if (IsUsingRealTime) {
                return Time.realtimeSinceStartup;
            }
            else {
                return Time.time;
            }
        }


        IEnumerator GoingToStartPlayingWhenReady(float preStartDelay, float duration, Action songStartPlayingCallback, Func<float> currentPlayingTimeGetter) {

            float startTime = GetCurrentTime();

            yield return new WaitUntil(() => IsReady && GetCurrentTime() - startTime > preStartDelay);

            StartPlaying(duration, songStartPlayingCallback, currentPlayingTimeGetter);
        }

        IEnumerator Playing (float duration, Action songStartPlayingCallback, Func<float> currentPlayingTimeGetter, float additionalPreDelay = 0f) {

            RoundStarted?.Invoke(this, EventArgs.Empty);
            Debug.Log("<Sheet> Round Start");
            

            additionalPreDelay = Mathf.Max(additionalPreDelay, 0f);

            IsPlaying = true;

            Queue<InteractableNoteOnSheet> playingInteractableNotesQueue = GetInteractableNotesOnSheetQueue(TracksDisplayController);
            List<InteractableNoteOnSheet> spawnedInteractableNotes = new List<InteractableNoteOnSheet>();

            float calculatedPreDelay = playingInteractableNotesQueue.Count > 0 ? Mathf.Max(PreOnsetDuration - playingInteractableNotesQueue.Peek().Onset, 0f) : 0f;

            float startPlayingTimeOfSong = GetCurrentTime() + calculatedPreDelay + additionalPreDelay;
            float currentPlayingTime = 0f;

            float GetCompensatedPlayingTime() {
                return currentPlayingTime + InputCompensationInSeconds;
            }

            bool isSongPlayed = false;


            TracksDisplayController?.ClearTracks();


            while (currentPlayingTime < duration) {
                
                currentPlayingTime =  currentPlayingTimeGetter();
                if (currentPlayingTime == 0f) {
                    currentPlayingTime = GetCurrentTime() - startPlayingTimeOfSong;
                } 
                
                
                if (!isSongPlayed && currentPlayingTime >= 0f) {
                    songStartPlayingCallback?.Invoke();
                    isSongPlayed = true;
                }

                // Check for note debut
                if (playingInteractableNotesQueue.Count > 0 && currentPlayingTime > GetTargetTimePositionDebutTimeFromPlayingStart(playingInteractableNotesQueue.Peek().Onset)) {
                    // Note debut
                    InteractableNoteOnSheet interactableNote = playingInteractableNotesQueue.Dequeue();
                    spawnedInteractableNotes.Add(interactableNote);

                    TracksDisplayController?.NoteDebut(interactableNote, GetProgressDistanceByDuration(interactableNote.Duration));
                }
                // Update spawned notes
                for (int i = 0 ; i < spawnedInteractableNotes.Count ; i++) {
                    InteractableNoteOnSheet interactableNote = spawnedInteractableNotes[i];

                    if (interactableNote.IsRetired) {
                        spawnedInteractableNotes.RemoveAt(i--);
                        continue;
                    }
                    

                    float noteOnsetProgressTimeRelativeToTargetTime = GetCompensatedPlayingTime() - interactableNote.Onset;
                    float noteOffsetProgressTimeRelativeToTargetTime = GetCompensatedPlayingTime() - interactableNote.Offset;


                    if (noteOffsetProgressTimeRelativeToTargetTime > _delayedDurationToDestroy) {
                        TracksDisplayController?.DestroyNote(interactableNote);
                        interactableNote.Retire();
                    }
                    else {
                        TracksDisplayController?.UpdateNotePosition(
                            interactableNote, 
                            GetTargetTimePositionTravellingProgress(interactableNote.Onset, currentPlayingTime), 
                            GetTargetTimePositionTravellingProgress(interactableNote.Offset, currentPlayingTime)
                        );

                        if (!interactableNote.HasRested) {

                            if (interactableNote is StrikableNoteOnSheet) {
                                StrikableNoteOnSheet strikableNote = (StrikableNoteOnSheet) interactableNote;

                                foreach (int trackIndex in strikableNote.RemainedOnTracksIndex) {

                                    // Player hit
                                    if (_playerStrikesChecker.HasStruckThisFrame(trackIndex)) {

                                        StrikeResult strikeResult = GetStrikeResult(noteOnsetProgressTimeRelativeToTargetTime);

                                        if (strikeResult != StrikeResult.None) {
                                            strikableNote.OnStruck(strikeResult, noteOnsetProgressTimeRelativeToTargetTime, trackIndex);
                                            _playerStrikesChecker.RemoveStrikeThisFrame(trackIndex);
                                        }
                                        else {
                                            if (strikableNote is InteractableTypeHoldNoteOnSheet) {
                                                InteractableTypeHoldNoteOnSheet typeHoldNote = (InteractableTypeHoldNoteOnSheet) strikableNote;

                                                if (noteOnsetProgressTimeRelativeToTargetTime > 0 && noteOffsetProgressTimeRelativeToTargetTime < 0) {
                                                    // Current playing time is in between note onset & offset
                                                    typeHoldNote.OnStrikeOnMidWay(trackIndex);
                                                    _playerStrikesChecker.RemoveStrikeThisFrame(trackIndex);
                                                }
                                            }
                                        }
                                    }

                                    // Player release
                                    if (_playerStrikesChecker.HasReleasedThisFrame(trackIndex)) {

                                        StrikeResult strikeResult = GetStrikeResult(noteOffsetProgressTimeRelativeToTargetTime);
                                        strikableNote.OnReleased(strikeResult, noteOffsetProgressTimeRelativeToTargetTime, trackIndex);
                                    }
                                }

                                // Note onset has reached the striking point
                                if (noteOnsetProgressTimeRelativeToTargetTime > 0) {
                                    if (strikableNote is InteractableTypeHoldNoteOnSheet) {
                                        InteractableTypeHoldNoteOnSheet typeHoldNote = (InteractableTypeHoldNoteOnSheet) strikableNote;

                                        if (typeHoldNote.HeadStruckResult == StrikeResult.None) {

                                            foreach (int trackIndex in typeHoldNote.RemainedOnTracksIndex) {
                                                if (_playerStrikesChecker.IsHolding(trackIndex)) {
                                                    strikableNote.OnStruck(MaxToleranceStrikeResult, noteOnsetProgressTimeRelativeToTargetTime, trackIndex);
                                                }
                                            }
                                        }
                                    }
                                }

                                // Note onset has escaped the striking zone
                                if (noteOnsetProgressTimeRelativeToTargetTime > MaxStrikeToleranceInSeconds) {

                                    if (strikableNote is InteractableTypeHoldNoteOnSheet) {
                                        InteractableTypeHoldNoteOnSheet typeHoldNote = (InteractableTypeHoldNoteOnSheet) strikableNote;

                                        if (!typeHoldNote.IsHeadMissed) {
                                            if (typeHoldNote.HeadStruckResult == StrikeResult.None) {
                                                typeHoldNote.OnHeadMissed();
                                            }
                                        }
                                    }
                                    else {
                                        if (!strikableNote.IsMissed) {
                                            strikableNote.OnMissed();
                                        }
                                    }

                                }

                                // Note offset has escaped the striking zone
                                if (noteOffsetProgressTimeRelativeToTargetTime > MaxStrikeToleranceInSeconds) {
                                    
                                    if (strikableNote is InteractableTypeHoldNoteOnSheet) {
                                        InteractableTypeHoldNoteOnSheet typeHoldNote = (InteractableTypeHoldNoteOnSheet) strikableNote;

                                        if (!typeHoldNote.IsSmashed) {
                                            if (typeHoldNote.IsBeingHolded) {
                                                typeHoldNote.OnOverHolded();
                                            }
                                            else {
                                                typeHoldNote.OnMissed();
                                            }
                                        }
                                    }
                                }
                                
                            }
                            else if (interactableNote is MonsterNoteOnSheet) {

                                MonsterNoteOnSheet monsterNote = (MonsterNoteOnSheet) interactableNote;

                                // Note has out-dated
                                if (!monsterNote.HasReachedDestination && noteOnsetProgressTimeRelativeToTargetTime > _monsterTolerancesInSeconds) {
                                    monsterNote.OnTargetPositionArrived(_currentPlayerPositionNumber);
                                }
                            }
                            
                        }

                    }


                    // Read result
                    if (!interactableNote.IsResultRead) {
                        if (interactableNote is StrikableNoteOnSheet) {

                            StrikableNoteOnSheet strikableNote = (StrikableNoteOnSheet) interactableNote;
                            if (strikableNote.IsSmashed) {

                                OnNoteStruck(strikableNote);
                                strikableNote.Rest();

                                interactableNote.IsResultRead = true;
                            }
                            else if (strikableNote.IsMissed) {
                                
                                OnNoteMissed(strikableNote);

                                interactableNote.IsResultRead = true;
                            }
                            else {

                                if (strikableNote is InteractableTypeHoldNoteOnSheet) {

                                    InteractableTypeHoldNoteOnSheet typeHoldNote = (InteractableTypeHoldNoteOnSheet) interactableNote;

                                    
                                    if (!typeHoldNote.IsHeadResultRead) {
                                        if (typeHoldNote.IsHeadMissed) {
                                            
                                            OnNoteMissed(typeHoldNote);

                                            typeHoldNote.IsHeadResultRead = true;
                                        }
                                        else if (typeHoldNote.HeadStruckResult != StrikeResult.None) {
                                            OnNoteStruck(typeHoldNote, typeHoldNote.HeadStruckResult);

                                            typeHoldNote.IsHeadResultRead = true;
                                        }
                                    }
                                    else {
                                        if (typeHoldNote.CurrentSegmentCounter < typeHoldNote.WholeSegmentsCount) {
                                            int currentSegmentIndex = (int) ( (GetCompensatedPlayingTime() - typeHoldNote.Onset) / TypeHoldNoteSegmentDuration );

                                            if (currentSegmentIndex > typeHoldNote.CurrentSegmentCounter) {
                                                
                                                if (typeHoldNote.CurrentSegmentCounter != -1) {
                                                    StrikeResult strikeResult = typeHoldNote.GetCurrentSegmentResult();

                                                    if (strikeResult == StrikeResult.None) {
                                                        OnNoteMissed(typeHoldNote);
                                                    }
                                                    else {
                                                        OnNoteStruck(typeHoldNote, strikeResult);
                                                    }
                                                }

                                                typeHoldNote.NextSegment();
                                            }
                                        }
                                    }

                                    // if (typeHoldNote.IsFailedToBeCompleted) {
                                    //     // OnTypeHoldNoteFailedToBeCompleted(typeHoldNote);
                                    //     // interactableNote.IsResultRead = true;
                                    // }

                                    // Display Control
                                    if (typeHoldNote.IsBeingHolded) {
                                        TracksDisplayController?.OnNoteHoldedThisFrame(interactableNote);
                                    }
                                }

                            }
                        }
                        else if (interactableNote is MonsterNoteOnSheet) {
                            
                            MonsterNoteOnSheet monsterNote = (MonsterNoteOnSheet) interactableNote;

                            if (monsterNote.HasReachedDestination) {

                                OnMonsterPassed(monsterNote);

                                interactableNote.IsResultRead = true;
                            }
                        }
                    }

                    
                }


                // Misses strikes
                foreach (int trackIndex in _playerStrikesChecker.GetTrackIndicesOfExistedStrikeThisFrame()) {
                    OnPlayerStrikeMissed(trackIndex);
                }
                _playerStrikesChecker.PerFrameReset();

                yield return null;
            }


            IsPlaying = false;
            RoundEnded?.Invoke(this, EventArgs.Empty);
            Debug.Log("<Sheet> Round Ended");



            #if UNITY_EDITOR
                if (loop) {
                    StartPlaying(duration, songStartPlayingCallback, currentPlayingTimeGetter);
                }
            #endif
        }


        void OnPlayerStrikeMissed (int trackIndex) {
            _playerStrikesChecker.StartCooldownCoroutine(trackIndex, this, GetCurrentTime);
            // _playerStrikesChecker.RemoveHolding(trackIndex);

            if (_showPlayerMissedStrikeLog) {
                Debug.Log($"Player's strike missed. (Track: {trackIndex})");
            }

            StrikeMissed?.Invoke(this, new TrackEventArgs{
                trackIndex = trackIndex
            });
        }

        void OnNoteStruck (StrikableNoteOnSheet strikableNote, StrikeResult overrideResult = StrikeResult.None) {

            foreach (int trackIndex in strikableNote.ExistedOnTracksIndex) {
                _playerStrikesChecker.StopCooldownCoroutine(trackIndex);
            }

            StrikeResult strikeResult = overrideResult == StrikeResult.None ? strikableNote.SmashedStrikeResult : overrideResult;

            TracksDisplayController?.OnNoteResulted(strikableNote, strikeResult.ToNoteResult());

            NoteStruck?.Invoke(this, new NoteStruckEventArgs {
                targetInteractableNote = strikableNote,
                strikeResult = strikeResult,
                deltaTime = strikableNote.SmashedDeltaTime
            });
        }

        void OnNoteMissed (StrikableNoteOnSheet strikableNote)  {

            TracksDisplayController?.OnNoteResulted(strikableNote, NoteResult.Miss);

            NoteMissed?.Invoke(this, new InteractableNoteResultedOnTrackEventArgs {
                targetInteractableNote = strikableNote
            });

            if (_showMissedNoteLog) {
                Debug.Log($"A type \"{strikableNote.NoteType}\" note has missed. ({ strikableNote.TrackInfoString })");
            }
        }


        void OnMonsterPassed (MonsterNoteOnSheet monsterNote) {

            MonsterPassResult monsterPassResult = MonsterPassResult.Safe;
            if (monsterNote.HasHitPlayer) {
                monsterPassResult = MonsterPassResult.Fail;
            }

            TracksDisplayController?.OnNoteResulted(monsterNote, monsterPassResult.ToNoteResult());

            MonsterPassed?.Invoke(this, new MonsterPassedEventArgs {
                targetInteractableNote = monsterNote,
                monsterPassResult = monsterPassResult
            });
        }
        #endregion

        #region Nested Classes/Structs
        // == Nested Classes/Structs ==
        [Serializable]
        protected class StrikeTolerance {
            const float TARGET_PROGRESS_TO_STRIKE = 1f;

            [SerializeField] StrikeResult _strikeResult;
            public StrikeResult StrikeResult => _strikeResult;

            [SerializeField] float _toleranceInSeconds;
            public float ToleranceInSeconds {
                get => _toleranceInSeconds;
                set => _toleranceInSeconds = value;
            }


            public static StrikeResult GetMaxToleranceStrikeResult (IEnumerable<StrikeTolerance> strikeTolerances) {
                StrikeResult result = StrikeResult.None;
                float maxToleranceInSeconds = 0f;
                foreach (StrikeTolerance tolerance in strikeTolerances) {
                    if (tolerance.ToleranceInSeconds > maxToleranceInSeconds) {
                        result = tolerance.StrikeResult;
                        maxToleranceInSeconds = tolerance.ToleranceInSeconds;
                    }
                }
                return result;
            }

            public static float GetMaxToleranceInSeconds (IEnumerable<StrikeTolerance> strikeTolerances) {
                return strikeTolerances.Max(strikeTolerance => strikeTolerance.ToleranceInSeconds);
            }

            public static StrikeTolerance[] GetSortedRangeLargeToSmall (IEnumerable<StrikeTolerance> strikeTolerances) {
                return strikeTolerances.OrderByDescending(strikeTolerance => strikeTolerance.ToleranceInSeconds).ToArray();
            }

        }

        protected class PlayerStrikesChecker {

            public event EventHandler<EventArgs> PlayerStruck;
            public event EventHandler<EventArgs> PlayerReleased;

            // == Private Fields ==
            InfoOnTrack[] _infoOnTracks;
            float _strikeCooldownDuration;

            // == Public Properties ==
            public int TrackCount => _infoOnTracks.Length;

            // == Public Methods ==
            public PlayerStrikesChecker (int trackCount, float strikeCooldownDuration) {
                _infoOnTracks = new InfoOnTrack[trackCount];
                for (int i = 0 ; i < _infoOnTracks.Length ; i++) {
                    _infoOnTracks[i] = new InfoOnTrack();

                    int trackIndex = i;
                    _infoOnTracks[i].Struck = () => PlayerStruck?.Invoke(this, new TrackEventArgs{ trackIndex = trackIndex });
                    _infoOnTracks[i].Released = () => PlayerReleased?.Invoke(this, new TrackEventArgs{ trackIndex = trackIndex });
                }

                _strikeCooldownDuration = strikeCooldownDuration;
            }

            public void PerFrameReset () {
                foreach (InfoOnTrack info in _infoOnTracks) {
                    info.PerFrameReset();
                }
            }

            public void AddStrikeThisFrame (int trackIndex) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].AddStrikeToThisFrame();
                }
            }

            public void AddReleaseThisFrame (int trackIndex) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].AddReleaseToThisFrame();
                }
            }

            public void RemoveStrikeThisFrame (int trackIndex) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].RemoveStrikeThisFrame();
                }
            }

            public void RemoveReleaseThisFrame (int trackIndex) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].RemoveReleaseThisFrame();
                }
            }

            // public void RemoveHolding (int trackIndex) {
            //     if (IsTrackIndexInRange(trackIndex)) {
            //         _infoOnTracks[trackIndex].RemoveHolding();
            //     }
            // }

            public bool HasStruckThisFrame (int trackInex) {
                return IsTrackIndexInRange(trackInex) && _infoOnTracks[trackInex].HasStruckThisFrame;
            }

            public bool HasReleasedThisFrame (int trackIndex) {
                return IsTrackIndexInRange(trackIndex) && _infoOnTracks[trackIndex].HasReleasedThisFrame;
            }

            public bool IsHolding (int trackIndex) {
                return IsTrackIndexInRange(trackIndex) && _infoOnTracks[trackIndex].IsHolding;
            }


            public void StartCooldownCoroutine (int trackIndex, MonoBehaviour coroutineCarrier, Func<float> currentTimeGetter) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].StartCoolingDownCoroutine(coroutineCarrier, _strikeCooldownDuration, currentTimeGetter);
                }
            }

            public void StopCooldownCoroutine (int trackIndex) {
                if (IsTrackIndexInRange(trackIndex)) {
                    _infoOnTracks[trackIndex].StopCurrentCoolingDown();
                }
            }

            public int[] GetTrackIndicesOfExistedStrikeThisFrame () {
                List<int> result = new List<int>();
                for (int i = 0 ; i < _infoOnTracks.Length ; i++) {
                    if (_infoOnTracks[i].HasStruckThisFrame) {
                        result.Add(i);
                    }
                }
                return result.ToArray();
            }

            public int[] GetTrackIndicesOfExistedReleaseThisFrame () {
                List<int> result = new List<int>();
                for (int i = 0 ; i < _infoOnTracks.Length ; i++) {
                    if (_infoOnTracks[i].HasReleasedThisFrame) {
                        result.Add(i);
                    }
                }
                return result.ToArray();
            }

            // == Protected Methods
            protected bool IsTrackIndexInRange (int trackIndex) {
                bool result = trackIndex >= 0 && trackIndex < TrackCount;
                if (!result) {
                    Debug.LogWarning("Track index is out of range!");
                }
                return result;
            }


            // == Nested Classes ==
            class InfoOnTrack {
                public Action Struck;
                public Action Released;

                public bool HasStruckThisFrame {get; private set;} = false;
                public bool HasReleasedThisFrame {get; private set;} = false;
                public bool IsHolding {get; private set;} = false;
                public bool IsCoolingDown {get; private set;} = false;

                MonoBehaviour _currentStrikeCoolingDownCoroutineCarrier = null;
                Coroutine _currentStrikeCoolingDownCoroutine = null;
  

                public void PerFrameReset () {
                    HasStruckThisFrame = false;
                    HasReleasedThisFrame = false;
                }

                public void AddStrikeToThisFrame () {
                    if (!IsCoolingDown) {
                        HasStruckThisFrame = true;
                        Struck?.Invoke();
                    }
                    IsHolding = true;
                }

                public void AddReleaseToThisFrame () {
                    if (IsHolding) {
                        HasReleasedThisFrame = true;
                        IsHolding = false;
                        Released?.Invoke();
                    }
                }

                public void RemoveStrikeThisFrame () {
                    HasStruckThisFrame = false;
                }

                public void RemoveReleaseThisFrame () {
                    HasReleasedThisFrame = false;
                }

                // public void RemoveHolding () {
                //     IsHolding = false;
                // }

                public void StartCoolingDownCoroutine (MonoBehaviour coroutineCarrier, float duration, Func<float> currentTimeGetter) {
                    StopCurrentStrikeCoolingDownCoroutine();
                    _currentStrikeCoolingDownCoroutine = coroutineCarrier.StartCoroutine(StrikeCoolingDown(duration, currentTimeGetter));
                    _currentStrikeCoolingDownCoroutineCarrier = coroutineCarrier;
                }

                public void StopCurrentCoolingDown () {
                    StopCurrentStrikeCoolingDownCoroutine();
                    IsCoolingDown = false;
                }

                
                protected void StopCurrentStrikeCoolingDownCoroutine () {
                    if (_currentStrikeCoolingDownCoroutine != null && _currentStrikeCoolingDownCoroutineCarrier) {
                        _currentStrikeCoolingDownCoroutineCarrier.StopCoroutine(_currentStrikeCoolingDownCoroutine);
                    }
                    _currentStrikeCoolingDownCoroutine = null;
                    _currentStrikeCoolingDownCoroutineCarrier = null;
                }

                protected IEnumerator StrikeCoolingDown (float duration, Func<float> currentTimeGetter) {
                    IsCoolingDown = true;
                    yield return CoroutineTools.CoolingDown(duration, currentTimeGetter);
                    IsCoolingDown = false;
                }
            }
        }

        protected abstract class InteractableNoteOnSheet : IInteractableNoteOnSheet {
            public Note.Type NoteType { get; private set; }
            public float Onset { get; private set; }
            public float Offset { get; protected set; }
            public float Duration => Offset - Onset;
            public int[] ExistedOnTracksIndex {
                get {
                    return (int[])_existedOnTracksIndex.Clone();
                }
            }
            public virtual int ScorableNoteCount => 1;

            public bool HasRested {get; private set;} = false;
            public bool IsRetired {get; private set;} = false;
            public virtual int[] RemainedOnTracksIndex => ExistedOnTracksIndex;
            public string TrackInfoString => $"Track: {string.Join(", ", ExistedOnTracksIndex)}";

            public bool IsResultRead { get; set; }


            protected int[] _existedOnTracksIndex;
            

            public InteractableNoteOnSheet(Note.Type noteType, float onset) {
                NoteType = noteType;
                Onset = onset;
                Offset = onset;
            }

            public void Rest () {
                HasRested = true;
            }

            public void Retire () {
                IsRetired = true;
            }
            
            
            
        }

        protected abstract class StrikableNoteOnSheet : InteractableNoteOnSheet {
            public bool IsSmashed {get; protected set;} = false;
            public StrikeResult SmashedStrikeResult {get; protected set;} = StrikeResult.None;
            public float SmashedDeltaTime {get; protected set;} = 0f;
            public bool IsMissed {get; protected set;} = false;

            public Action<StrikeResult> OnHitCallback {get; set;} = null;
            public Action<StrikeResult> OnReleasedCallback {get; set;} = null;
            public Action               OnOverHoldingCallback {get; set;} = null;


            public StrikableNoteOnSheet(Note.Type noteType, float onset) : base(noteType, onset) {}

            public abstract void OnStruck (StrikeResult strikeResult, float deltaTime, int trackIndex = -1);

            public abstract void OnReleased (StrikeResult strikeResult, float deltaTime, int trackIndex = -1);

            public virtual void OnMissed () {
                if (!IsSmashed) {
                    IsMissed = true;
                }
            }


            protected void OnSmashed (StrikeResult strikeResult, float deltaTime) {
                IsSmashed = true;
                SmashedStrikeResult = strikeResult;
                SmashedDeltaTime = deltaTime;
            }

            public class SmashedEventArgs : EventArgs {
                public StrikeResult strikeResult;
            }
        }

        protected class InteractableTypeTapNoteOnSheet : StrikableNoteOnSheet {

            public InteractableTypeTapNoteOnSheet (Note.Type noteType, float onset, int trackIndex) : base (noteType, onset) {
                _existedOnTracksIndex = new int[] { trackIndex };
            }

            public override void OnStruck (StrikeResult strikeResult, float deltaTime, int trackIndex = -1) {

                OnHitCallback?.Invoke(strikeResult);
                OnReleasedCallback?.Invoke(strikeResult);
                OnSmashed(strikeResult, deltaTime);
            }

            public override void OnReleased (StrikeResult strikeResult, float deltaTime, int trackIndex = -1) {
                
            }

            public override void OnMissed() {
                base.OnMissed();
            }
        }

        protected class InteractableTypeHoldNoteOnSheet : StrikableNoteOnSheet {

            public override int ScorableNoteCount => WholeSegmentsCount + 2;

            public bool IsHeadResultRead { get; set; } = false;
            public bool IsHeadMissed { get; protected set; } = false;
            public StrikeResult HeadStruckResult { get; protected set; } = StrikeResult.None;

            public bool IsBeingHolded { get; set; } = false;

            public int WholeSegmentsCount { get; protected set; } = 0;

            public int CurrentSegmentCounter { get; protected set; } = -1;
            public bool IsCurrentSegmentKeptHolded { get; protected set; } = false;

            public InteractableTypeHoldNoteOnSheet (Note.Type noteType, float onset, float offset, int trackIndex, int wholeSegmentsCount) : base(noteType, onset) {
                Offset = offset;
                _existedOnTracksIndex = new int[] { trackIndex };
                WholeSegmentsCount = wholeSegmentsCount;
            }

            public override void OnStruck (StrikeResult strikeResult, float deltaTime, int trackIndex = -1) {

                if (!IsBeingHolded) {
                    IsBeingHolded = true;
                    HeadStruckResult = strikeResult;

                    // Callback
                    OnHitCallback?.Invoke(strikeResult);
                }
            }

            public void OnStrikeOnMidWay(int trackIndex = -1) {
                IsBeingHolded = true;
            }
            
            public override void OnReleased (StrikeResult strikeResult, float deltaTime, int trackIndex = -1) {
                if (IsBeingHolded) {
                    IsBeingHolded = false;

                    OnReleasedCallback?.Invoke(strikeResult);

                    // Released on the way of segment
                    if (strikeResult == StrikeResult.None) {
                        IsCurrentSegmentKeptHolded = false;
                    }
                    // Success to complete
                    else {
                        if (IsCurrentSegmentKeptHolded) {
                            OnSmashed(strikeResult, deltaTime);
                        }
                    }

                }
            }

            public void OnHeadMissed () {
                IsHeadMissed = true;
            }

            public StrikeResult GetCurrentSegmentResult () {
                if (IsCurrentSegmentKeptHolded) {
                    return StrikeResult.Perfect;
                }
                return StrikeResult.None;
            }

            public void NextSegment () {
                CurrentSegmentCounter++;
                IsCurrentSegmentKeptHolded = IsBeingHolded;
            }

            public void OnOverHolded () {
                if (!IsSmashed) {
                    IsBeingHolded = false;
                    IsMissed = true;

                    // Callback
                    OnOverHoldingCallback?.Invoke();
                }
            }
        }

        protected class InteractableTypeTapCombinationNoteOnSheet : StrikableNoteOnSheet {

            public override int[] RemainedOnTracksIndex {
                get {
                    List<int> result = new List<int>();

                    for (int i = 0 ; i < _isStruckOfExistedTracks.Length ; i++) {
                        if (!_isStruckOfExistedTracks[i]) {
                            result.Add(_existedOnTracksIndex[i]);
                        }
                    }
                    return result.ToArray();
                }
            }


            bool[] _isStruckOfExistedTracks;
            StrikeResult[] _strikeResultOfExisedTracks;

            public InteractableTypeTapCombinationNoteOnSheet (Note.Type noteType, float onset, int[] tracksIndex) : base(noteType, onset) {
                _existedOnTracksIndex = (int[]) tracksIndex.Clone();
                _isStruckOfExistedTracks = new bool[_existedOnTracksIndex.Length];
                _strikeResultOfExisedTracks = new StrikeResult[_existedOnTracksIndex.Length];
            }

            public override void OnStruck (StrikeResult strikeResult, float deltaTime, int trackIndex) {
                
                int indexOfExistedTracks = Array.IndexOf(_existedOnTracksIndex, trackIndex);

                if (indexOfExistedTracks >= 0 && indexOfExistedTracks < _isStruckOfExistedTracks.Length) {
                    _isStruckOfExistedTracks[indexOfExistedTracks] = true;
                    _strikeResultOfExisedTracks[indexOfExistedTracks] = strikeResult;
                }

                SmashedDeltaTime = deltaTime;

                CheckSmashed();
            }

            public override void OnReleased (StrikeResult strikeResult, float deltaTime, int trackIndex = -1) {
                
            }

            protected void CheckSmashed () {
                // Check if smashed
                bool isSmashed = true;
                foreach (bool isStruck in _isStruckOfExistedTracks) {
                    if (!isStruck) {
                        isSmashed = false;
                        break;
                    }
                }

                if (isSmashed) {
                    StrikeResult resultStrikeResult = StrikeResult.Perfect;
                    foreach (StrikeResult strikeResult in _strikeResultOfExisedTracks) {
                        if (resultStrikeResult == StrikeResult.Perfect && strikeResult == StrikeResult.Good) {
                            resultStrikeResult = StrikeResult.Good;
                        }
                        else if (strikeResult == StrikeResult.None) {
                            resultStrikeResult = StrikeResult.None;
                        }
                    }

                    OnHitCallback?.Invoke(resultStrikeResult);
                    OnReleasedCallback?.Invoke(resultStrikeResult);
                    OnSmashed(resultStrikeResult, SmashedDeltaTime);
                }
            }
        }

        protected class MonsterNoteOnSheet : InteractableNoteOnSheet {

            public bool HasReachedDestination {get; protected set;} = false;
            public bool HasHitPlayer {get; protected set;} = false;

            public MonsterNoteOnSheet (Note.Type noteType, float onset, int trackIndex) : base(noteType, onset) {
                _existedOnTracksIndex = new int[] { trackIndex };
            }

            public void OnTargetPositionArrived (int playerLocatedTrackIndex) {
                HasReachedDestination = true;
                HasHitPlayer = Array.Exists(ExistedOnTracksIndex, index => index == playerLocatedTrackIndex);
            }
        }

        public class InteractableNoteResultedOnTrackEventArgs : EventArgs {
            public IInteractableNoteOnSheet targetInteractableNote;
        }

        public class NoteStruckEventArgs : InteractableNoteResultedOnTrackEventArgs {
            public StrikeResult strikeResult;
            public float deltaTime;
        }

        public class MonsterPassedEventArgs : InteractableNoteResultedOnTrackEventArgs {
            public MonsterPassResult monsterPassResult;
        }

        public class TrackEventArgs : EventArgs {
            public int trackIndex;
        }

    }
    #endregion
}
