using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectComet.OptionsHandling.Display {

    public class OptionsDisplayTextProvider : MonoBehaviour {

        [SerializeField] InputActionAsset _gamePlayActions;


        protected OptionsHolder optionsHolder => OptionsHolder.current;


        public string GetDisplayText (string key, bool resetToCurrent = false) {
            if (optionsHolder) {
                switch (key) {
                    case "NoteSpeed": {
                        return optionsHolder.CurrentOptions.NoteSpeedRate.ToString("0.0");
                    }
                    case "InputTimingCompensation": {
                        return ( (int) -Mathf.Round((optionsHolder.CurrentOptions.InputTimingCompensation / OptionsHolder.InputTimingCompensationStep)) ).ToString();
                    }
                    case "MusicVolume": {
                        return optionsHolder.CurrentOptions.MusicVolume.ToString();
                    }
                    case "SFXVolume": {
                        return optionsHolder.CurrentOptions.SFXVolume.ToString();
                    }
                    case "ScreenMode": {
                        return optionsHolder.CurrentOptions.GetFullscreenValue(resetToCurrent) ? "FULLSCREEN" : "WINDOWED";
                    }
                    case "Resolution": {
                        Vector2Int res = optionsHolder.CurrentOptions.GetResolutaionValue(resetToCurrent);
                        return $"{res.x} X {res.y}";
                    }
                }
            }

            if (_gamePlayActions) {
                InputAction inputAction = _gamePlayActions.FindAction(key);
                if (inputAction != null) {
                    return inputAction.GetBindingDisplayString();
                }
            }
            

            return string.Empty;
        }

    }

}
