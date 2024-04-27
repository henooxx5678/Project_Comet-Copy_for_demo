using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityUI {

    [RequireComponent(typeof(Text))]
    public class TextEventEmitter : MonoBehaviour {
        
        public UnityEvent<string> onTextChange;


        Text _targetText;
        protected Text targetText => _targetText ? _targetText : _targetText = GetComponent<Text>();


        string _prevText = null;

        protected virtual void Update () {
            string currentText = targetText.text;

            if (currentText != _prevText) {
                onTextChange.Invoke(currentText);
            }

            _prevText = currentText;
        }        

    }

}