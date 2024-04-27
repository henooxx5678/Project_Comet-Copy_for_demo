using System;
using UnityEngine;
using DG.Tweening;

namespace ProjectComet.CoreGameplay.Display {
    
    public class ShakingPositionHandler : MonoBehaviour {
        

        [SerializeField] TargetType _targetType;

        [SerializeField] ShakingParameters _shakingParameters;

        Tween _shakingTween;



        public void Shake () {

            if (_shakingTween != null && _shakingTween.IsActive() && _shakingTween.IsPlaying()) {
                _shakingTween.Complete();
            }

            switch (_targetType) {
                case TargetType.Transform: {
                
                    _shakingTween = this.transform.DOShakePosition(
                        _shakingParameters.duration,
                        _shakingParameters.strengh,
                        _shakingParameters.vibrato,
                        _shakingParameters.randomness,
                        _shakingParameters.fadeout
                    );
                    break;
                }
                case TargetType.Camera: {
                    Camera cam = this.GetComponent<Camera>();

                    if (cam) {
                        _shakingTween = cam.DOShakePosition(
                            _shakingParameters.duration,
                            _shakingParameters.strengh,
                            _shakingParameters.vibrato,
                            _shakingParameters.randomness,
                            _shakingParameters.fadeout
                        );
                    }
                    break;
                }
            }
            
        }


        [Serializable]
        public struct ShakingParameters {
            public float duration;
            public Vector3 strengh;
            public int vibrato;
            public float randomness;
            public bool fadeout;
        }

        public enum TargetType {
            Transform,
            Camera
        }
 
    }

}