using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectComet.CoreGameplay.Player {

    public class LevelPlayingActionsHandler : MonoBehaviour {

        [SerializeField] PlayerStatus _playerStatus;
        [SerializeField] Sheet _currentInteractedSheet;
        public Sheet CurrentInteractedSheet {
            get => _currentInteractedSheet;
            protected set => _currentInteractedSheet = value;
        }
    

        public void PerformAction (ActionType actionType) {
            switch (actionType) {
                case ActionType.StrikeLeft:
                    CurrentInteractedSheet?.OnPlayerStrike(0);
                    break;

                case ActionType.StrikeMid:
                    CurrentInteractedSheet?.OnPlayerStrike(1);
                    break;

                case ActionType.StrikeRight:
                    CurrentInteractedSheet?.OnPlayerStrike(2);
                    break;

                case ActionType.ReleaseLeft:
                    CurrentInteractedSheet?.OnPlayerRelease(0);
                    break;

                case ActionType.ReleaseMid:
                    CurrentInteractedSheet?.OnPlayerRelease(1);
                    break;

                case ActionType.ReleaseRight:
                    CurrentInteractedSheet?.OnPlayerRelease(2);
                    break;

                case ActionType.MoveLeft:
                    _playerStatus.OnMoveAction(-1, true);
                    break;

                case ActionType.MoveRight:
                    _playerStatus.OnMoveAction(1, true);
                    break;

                case ActionType.MoveLeftOffset:
                    _playerStatus.OnMoveAction(-1, false);
                    break;

                case ActionType.MoveRightOffset:
                    _playerStatus.OnMoveAction(1, false);
                    break;
            }
        }



        public enum ActionType {
            None,
            StrikeLeft,
            StrikeMid,
            StrikeRight,
            ReleaseLeft,
            ReleaseMid,
            ReleaseRight,
            MoveLeft,
            MoveRight,
            MoveLeftOffset,
            MoveRightOffset
        }

    }

}
