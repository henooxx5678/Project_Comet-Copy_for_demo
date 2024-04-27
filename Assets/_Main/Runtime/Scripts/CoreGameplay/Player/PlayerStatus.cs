using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DoubleHeat.Utilities;

namespace ProjectComet.CoreGameplay.Player {

    [DisallowMultipleComponent]
    public class PlayerStatus : MonoBehaviour {

        const int MIN_MOVEMENT_POSITION_NUMBER = -1;
        const int MAX_MOVEMENT_POSITION_NUMBER = 1;

        public event EventHandler<EventArgs> MovementPositionChanged;

        public UnityEvent onMoveLeft;
        public UnityEvent onMoveRight;


        [SerializeField] MovementType _movementType;
        [SerializeField] float _moveActionCooldown = 0.12f;


        int _currentMovementPositionNumber = 0;
        public int CurrentMovementPositionNumber {
            get {
                return _currentMovementPositionNumber;
            }
            protected set {
                if (_currentMovementPositionNumber != value) {
                    int prevPosNum = _currentMovementPositionNumber;
                    _currentMovementPositionNumber = value;

                    OnMovementPositionChanged(_currentMovementPositionNumber - prevPosNum);
                }
            }
        }

        public bool IsMoveActionCoolingDown {get; protected set;} = false;

        Coroutine _currentCoolingDownCoroutine = null;
        protected Coroutine currentCoolingDownCoroutine {
            get {
                return _currentCoolingDownCoroutine;
            }
            set {
                if (_currentCoolingDownCoroutine != null) {
                    StopCoroutine(_currentCoolingDownCoroutine);
                }
                _currentCoolingDownCoroutine = value;
            }
        }

        // Been used when _movementType == MovementType.PressGoReleaseBack
        OneFrameMovementInfo _movementInfoThisFrame = new OneFrameMovementInfo();


        void Update () {

            switch (_movementType) {

                case MovementType.PressGoReleaseBack:
                    if (_movementInfoThisFrame.IsBackingToCenter) {
                        CurrentMovementPositionNumber = 0;
                    }
                    else {
                        CurrentMovementPositionNumber = _movementInfoThisFrame.OnsetMovementDestination;
                    }

                    _movementInfoThisFrame.Reset(CurrentMovementPositionNumber);
                    break;
            }
        }


        public void OnMoveAction (int leftOrRightDirection, bool isOnset = true) {

            if (IsMoveActionCoolingDown)
                return;

            switch (_movementType) {
                case MovementType.PressToGo:
                    if (isOnset) {
                        int nextMovementPositionNumber = CurrentMovementPositionNumber + leftOrRightDirection;
                        nextMovementPositionNumber = Math.Max(nextMovementPositionNumber, MIN_MOVEMENT_POSITION_NUMBER);
                        nextMovementPositionNumber = Math.Min(nextMovementPositionNumber, MAX_MOVEMENT_POSITION_NUMBER);
                        CurrentMovementPositionNumber = nextMovementPositionNumber;
                    }
                    break;

                case MovementType.PressGoReleaseBack:
                    if (isOnset) {
                        if (Math.Abs(CurrentMovementPositionNumber - leftOrRightDirection) < 2) {
                            _movementInfoThisFrame.OnsetMovementDestination += leftOrRightDirection;
                        }
                    }
                    else {
                        if (CurrentMovementPositionNumber == leftOrRightDirection) {
                            _movementInfoThisFrame.IsBackingToCenter = true;
                        }
                    }
                    break;
            }

        }


        IEnumerator MoveActionCooldownCoroutine () {
            IsMoveActionCoolingDown = true;
            yield return StartCoroutine(CoroutineTools.CoolingDown(_moveActionCooldown, () => Time.time));
            IsMoveActionCoolingDown = false;
        }

        void OnMovementPositionChanged (int deltaPositionNumber) {
            currentCoolingDownCoroutine = StartCoroutine(MoveActionCooldownCoroutine());

            MovementPositionChanged?.Invoke(this, new MovementPositionChangedEventArgs {
                newPositionNumber = _currentMovementPositionNumber
            });

            if (deltaPositionNumber < 0) {
                onMoveLeft?.Invoke();
            }
            else {
                onMoveRight?.Invoke();
            }
        }


        public class OneFrameMovementInfo {
            int _onsetMovementDestination = 0;
            public int OnsetMovementDestination {
                get {
                    return _onsetMovementDestination;
                }
                set {
                    _onsetMovementDestination = value;
                    _onsetMovementDestination = Math.Min(_onsetMovementDestination, 1);
                    _onsetMovementDestination = Math.Max(_onsetMovementDestination, -1);
                }
            }

            public bool IsBackingToCenter {get; set;}

            public void Reset (int currentMovementPosition) {
                OnsetMovementDestination = currentMovementPosition;
                IsBackingToCenter = false;
            }

            public void Clear () {
                OnsetMovementDestination = 0;
                IsBackingToCenter = false;
            }
        }

        public class MovementPositionChangedEventArgs : EventArgs {
            public int newPositionNumber;
        }


        public enum MovementType {
            PressToGo,
            PressGoReleaseBack
        }

    }

}
