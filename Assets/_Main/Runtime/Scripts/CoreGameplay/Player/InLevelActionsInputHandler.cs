using UnityEngine;
using UnityEngine.InputSystem;
using ProjectComet.PlayerInputs;

namespace ProjectComet.CoreGameplay.Player {

    [RequireComponent(typeof(PlayerInput))]
    public class InLevelActionsInputHandler : LevelActionsInputHandler {


        protected override void OnPlayerInputActionTriggered(InputAction.CallbackContext ctx) {
            base.OnPlayerInputActionTriggered(ctx);

            InputAction action = ctx.action;

            if (ctx.performed) {
                switch (action.actionMap.name) {
                    
                    case "Playing": {
                        switch (action.name) {
                            case "Pause": {
                                if (levelPlayingStatesManager) {
                                    levelPlayingStatesManager.Pause();
                                }
                                break;
                            }
                        }
                        break;
                    }
                    case "Paused": {
                        switch (action.name) {
                            case "Resume": {
                                if (levelPlayingStatesManager) {
                                    levelPlayingStatesManager.Resume();
                                }
                                break;
                            }
                            case "Restart": {

                                break;
                            }
                            case "Exit": {
                                break;
                            }
                        }
                        break;
                    }

                }
            }
        }



    }
}
