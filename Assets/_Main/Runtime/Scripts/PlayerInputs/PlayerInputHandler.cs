using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectComet.PlayerInputs {

    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour {


        PlayerInput _playerInput;
        protected PlayerInput playerInput {
            get {
                _playerInput ??= GetComponent<PlayerInput>();
                return _playerInput;
            }
        }


        [Header("Debug")]
        [SerializeField] bool _showInputLog = false;


        protected virtual void OnEnable () {
            if (playerInput) {
                playerInput.onActionTriggered += OnPlayerInputActionTriggered;
            }
            else {
                Debug.Log("\"PlayerInput\" not found.");
            }
        }

        protected virtual void OnDisable() {
            if (playerInput) {
                playerInput.onActionTriggered -= OnPlayerInputActionTriggered;
            }
        }


        protected virtual void OnPlayerInputActionTriggered (InputAction.CallbackContext ctx) {
            InputAction action = ctx.action;

            if (_showInputLog) {
                if (ctx.performed) {
                    Debug.Log($"Player performed action: \"{action.name}\" ({action.actionMap.name})");
                }

                if (ctx.canceled) {
                    Debug.Log($"Player canceled action: \"{action.name}\" ({action.actionMap.name})");
                }
            }
            
        }

    }

}
