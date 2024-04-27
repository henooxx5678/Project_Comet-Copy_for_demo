using UnityEngine;
using UnityEngine.InputSystem;
using ProjectComet.PlayerInputs;

namespace ProjectComet.CoreGameplay.Player {

    [RequireComponent(typeof(PlayerInput))]
    public class LevelPlayingActionsInputHandler : LevelActionsInputHandler {

        [Header("REFS")]
        [SerializeField] LevelPlayingActionsHandler _levelPlayingActionsHandler;

        protected override void OnPlayerInputActionTriggered(InputAction.CallbackContext ctx) {
            base.OnPlayerInputActionTriggered(ctx);

            InputAction action = ctx.action;

            switch(action.actionMap.name) {
                case "Playing": {

                    LevelPlayingActionsHandler.ActionType actionType = LevelPlayingActionsHandler.ActionType.None;


                    if (ctx.performed) {
                        switch (action.name) {

                            case "Strike Left":
                                actionType = LevelPlayingActionsHandler.ActionType.StrikeLeft;
                                break;

                            case "Strike Mid":
                                actionType = LevelPlayingActionsHandler.ActionType.StrikeMid;
                                break;

                            case "Strike Right":
                                actionType = LevelPlayingActionsHandler.ActionType.StrikeRight;
                                break;

                            case "Move Left":
                                actionType = LevelPlayingActionsHandler.ActionType.MoveLeft;
                                break;

                            case "Move Right":
                                actionType = LevelPlayingActionsHandler.ActionType.MoveRight;
                                break;

                            case "Pause":
                                if (levelPlayingStatesManager) {
                                    levelPlayingStatesManager.Pause();
                                }
                                break;
                            
                        }
                    }

                    if (ctx.canceled) {
                        switch (action.name) {

                            case "Strike Left":
                                actionType = LevelPlayingActionsHandler.ActionType.ReleaseLeft;
                                break;

                            case "Strike Mid":
                                actionType = LevelPlayingActionsHandler.ActionType.ReleaseMid;
                                break;

                            case "Strike Right":
                                actionType = LevelPlayingActionsHandler.ActionType.ReleaseRight;
                                break;

                            case "Move Left":
                                actionType = LevelPlayingActionsHandler.ActionType.MoveLeftOffset;
                                break;

                            case "Move Right":
                                actionType = LevelPlayingActionsHandler.ActionType.MoveRightOffset;
                                break;
                        }
                    }

                    if (_levelPlayingActionsHandler) {
                        _levelPlayingActionsHandler.PerformAction(actionType);
                    }

                    break;
                }
                case "Paused": {
                    if (ctx.performed) {
                        // switch (action.name) {

                        // }
                    }
                    break;
                }
            }
        }




    }
}
