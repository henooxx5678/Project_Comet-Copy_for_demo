using System;
using UnityEngine;
using DoubleHeat.UI;

namespace ProjectComet.OptionsHandling.Display {

    public class OptionsTextDisplayController : MonoBehaviour {


        [SerializeField] OptionsDisplayTextProvider _optionsDisplayTextProvider;
        [SerializeField] TextDisplayInfo[] _textDisplayInfos;

        protected virtual void OnEnable () {
            SetDisplay(true);
            
            if (OptionsHolder.current) {
                OptionsHolder.current.ResetEvent += OnReset;
            }
        }

        protected virtual void OnDisable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.ResetEvent -= OnReset;
            }
        }


        public void UpdateDisplay () {
            SetDisplay(false);
        }

        
        protected void SetDisplay (bool resetToCurrent) {
            if (_optionsDisplayTextProvider && _textDisplayInfos != null) {
                foreach (TextDisplayInfo info in _textDisplayInfos) {
                    info.SetText(_optionsDisplayTextProvider.GetDisplayText(info.Key, resetToCurrent));
                }
            }
        }


        protected void OnReset () {
            SetDisplay(true);
        }



        [Serializable]
        protected class TextDisplayInfo {
            [SerializeField] string _key;
            public string Key => _key;

            [SerializeField] Component _textDisplay;


            public void SetText (string text) {
                TextDisplayTools.SetTextOfTextDisplayComponent(_textDisplay, text);
            }
            
        }

    }

}