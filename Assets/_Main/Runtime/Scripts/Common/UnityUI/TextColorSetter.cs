using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace ProjectComet.Common.UnityUI {

    
    public class TextColorSetter : MonoBehaviour {
        Text _text;
        public Text TargetText => _text ??= GetComponent<Text>();


        [SerializeField] Color[] _colors;

    
        bool _hasSet = false;
        Color _currentColor = default;


        protected virtual void LateUpdate () {
            if (_hasSet) {
                if (TargetText) {
                    TargetText.color = _currentColor;
                }
            }
        }


        public void SetColorByIndex (int index) {
            if (index >= 0 && index < _colors.Length) {
                _currentColor = _colors[index];
                _hasSet = true;
            }
        }

    }
}
