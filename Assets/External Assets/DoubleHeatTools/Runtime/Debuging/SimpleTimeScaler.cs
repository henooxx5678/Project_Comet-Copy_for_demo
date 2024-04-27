using UnityEngine;

namespace DoubleHeat.Debuging {
    
    public class SimpleTimeScaler : MonoBehaviour {
        
        [Header("Slower")]
        [Range(0f, 2f)]
        [SerializeField] float _timeScale = 1f;
        [SerializeField] bool _resetWhenPlay = true;

        [Header("Output")]
        [SerializeField] float _currentTimeScale = 1f;


        protected virtual void Awake () {
            if (_resetWhenPlay) {
                _timeScale = 1f;
            }
        }

        protected virtual void OnValidate () {
            Time.timeScale = _timeScale;
        }

        protected virtual void Update () {
            _currentTimeScale = Time.timeScale;
        }

    }

}