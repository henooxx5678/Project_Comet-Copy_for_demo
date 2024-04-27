using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ProjectComet.Common.UnityUI {

    public class NavigationHandler : MonoBehaviour {
        
        const string _uiActionMapName = "UI";

        [SerializeField] InputActionAsset _inputActionAsset;
        [SerializeField] string _navigationActionName = "Navigate";

        public UnityEvent<Vector2> onNavigate;

        protected virtual void OnEnable () {
            if (_inputActionAsset) {
                var actionMap = _inputActionAsset.FindActionMap(_uiActionMapName);
                if (actionMap != null) {
                    actionMap.actionTriggered += OnUIActionTriggered;
                }
            }
        }

        protected virtual void OnDisable () {
            if (_inputActionAsset) {
                var actionMap = _inputActionAsset.FindActionMap(_uiActionMapName);
                if (actionMap != null) {
                    actionMap.actionTriggered -= OnUIActionTriggered;
                }
            }
        }


        protected void OnUIActionTriggered (InputAction.CallbackContext ctx) {
            if (ctx.performed) {
                if (ctx.action.name == _navigationActionName) {
                    OnNavigate(ctx.ReadValue<Vector2>());
                }
            }
        }


        protected virtual void OnNavigate (Vector2 value) {
            onNavigate?.Invoke(value);
        }

    }
}