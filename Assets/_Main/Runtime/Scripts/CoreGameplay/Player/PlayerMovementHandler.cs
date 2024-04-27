using System;
using UnityEngine;
using DG.Tweening;

namespace ProjectComet.CoreGameplay.Player {

    [RequireComponent(typeof(PlayerStatus))]
    public class PlayerMovementHandler : MonoBehaviour {

        PlayerStatus _playerStatus;
        protected PlayerStatus playerStatus {
            get {
                _playerStatus ??= GetComponent<PlayerStatus>();
                return _playerStatus;
            }
        }


        [SerializeField] float _transitionDuration = 0.3f;

        [Header("REFS")]
        [SerializeField] Component _tracksDisplayControllerComponentType;
        ITracksDisplayController _tracksDisplayController = null;
        protected ITracksDisplayController tracksDisplayController {
            get {
                if (_tracksDisplayController == null && _tracksDisplayControllerComponentType is ITracksDisplayController) {
                    _tracksDisplayController = (ITracksDisplayController)_tracksDisplayControllerComponentType;
                }
                return _tracksDisplayController;
            }
        }


        Vector3 _initPosition = Vector3.zero;
        Tween _currentMovingTween = null;
        protected Tween currentMovingTween {
            get {
                return _currentMovingTween;
            }
            set {
                _currentMovingTween?.Kill(false);
                _currentMovingTween = value;
            }
        }



        void Awake () {
            _initPosition = transform.position;
        }

        void OnEnable () {
            if (playerStatus != null) {
                playerStatus.MovementPositionChanged += OnPlayerMovementPositionChanged;
            }
        }

        void OnDisable () {
            if (playerStatus != null) {
                playerStatus.MovementPositionChanged -= OnPlayerMovementPositionChanged;
            }
        }




        void OnPlayerMovementPositionChanged (object sender, EventArgs args) {
            if (args is PlayerStatus.MovementPositionChangedEventArgs) {
                PlayerStatus.MovementPositionChangedEventArgs movementPositionChangedEventArgs = (PlayerStatus.MovementPositionChangedEventArgs) args;

                if (tracksDisplayController != null && tracksDisplayController.IsExists) {
                    Vector3 nextPosition = _initPosition + tracksDisplayController.RightStepVector * movementPositionChangedEventArgs.newPositionNumber;
                    currentMovingTween = transform.DOMove(nextPosition, _transitionDuration);
                }
                else {
                    Debug.LogError("Cannot access target TracksDisplayController1");
                }
            }
        }



    }
}
