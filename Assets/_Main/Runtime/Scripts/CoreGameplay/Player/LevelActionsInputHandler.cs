using UnityEngine;
using UnityEngine.InputSystem;
using ProjectComet.PlayerInputs;

namespace ProjectComet.CoreGameplay.Player {

    [RequireComponent(typeof(PlayerInput))]
    public class LevelActionsInputHandler : PlayerInputHandler {

        [Header("Level Playing States")]
        [SerializeField] LevelPlayingStatesManager _levelPlayingStatesManager;
        protected LevelPlayingStatesManager levelPlayingStatesManager => _levelPlayingStatesManager;

        protected override void OnEnable() {
            base.OnEnable();

            if (_levelPlayingStatesManager) {
                _levelPlayingStatesManager.CurrentPausingStatus.CurrentPlayingStateChanged += OnLevelPlayingStateChanged;
            }
        }

        protected override void OnDisable() {
            base.OnDisable();

            if (_levelPlayingStatesManager) {
                _levelPlayingStatesManager.CurrentPausingStatus.CurrentPlayingStateChanged -= OnLevelPlayingStateChanged;
            }
        }


        protected virtual void OnLevelPlayingStateChanged(LevelPlayingStatesManager.PlayingState playingState) {
            switch (playingState) {
                case LevelPlayingStatesManager.PlayingState.Paused: {
                    playerInput.SwitchCurrentActionMap("Paused");
                    break;
                }
                case LevelPlayingStatesManager.PlayingState.Playing: {
                    playerInput.SwitchCurrentActionMap("Playing");
                    break;
                }
                case LevelPlayingStatesManager.PlayingState.Ended: {
                    playerInput.SwitchCurrentActionMap("Ended");
                    break;
                }
            }
        }

    }
}
