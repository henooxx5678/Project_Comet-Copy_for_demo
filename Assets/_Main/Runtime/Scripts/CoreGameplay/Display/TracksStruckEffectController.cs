using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DoubleHeat.Utilities;

namespace ProjectComet.CoreGameplay.Display {

    public class TracksStruckEffectController : MonoBehaviour {

        [SerializeField] float _releaseEffectDuration = 0.1f;
        [SerializeField] Ease _releaseEffectEase;

        [Header("REFS")]
        [SerializeField] GameObject[] _strikeEffectOfTracks;

        Tween[] _releaseEffectTweenOfTracks;
        protected IList<Tween> releaseEffectTweenOfTracks => _releaseEffectTweenOfTracks ??= new Tween[_strikeEffectOfTracks.Length];


        bool[] _isHoldingOfTracks;
        protected bool[] isHoldingOfTracks {
            get {
                return _isHoldingOfTracks ??= new bool[_strikeEffectOfTracks.Length];
            }
        }


        public void OnStrikeStarted (int trackIndex) {
            SetStrikeEffectOn(trackIndex);
        }

        public void OnStrikeReleased (int trackIndex) {
            PlayReleaseEffect(trackIndex);
        }


        protected void SetStrikeEffectOn (int trackIndex) {

            if (trackIndex < releaseEffectTweenOfTracks.Count) {
                releaseEffectTweenOfTracks[trackIndex]?.Kill(false);
            }

            if (trackIndex >= 0 && trackIndex < _strikeEffectOfTracks.Length) {

                isHoldingOfTracks[trackIndex] = true;

                GameObject targetStrikeEffect = _strikeEffectOfTracks[trackIndex];

                if (targetStrikeEffect) {
                    targetStrikeEffect.SetActive(true);

                    SpriteRenderer spriteRenderer = targetStrikeEffect.GetComponent<SpriteRenderer>();
                    if (spriteRenderer) {
                        spriteRenderer.color = spriteRenderer.color.GetAfterSetA(1f);
                    }
                }

            }
        }

        protected void PlayReleaseEffect (int trackIndex) {
            if (trackIndex >= 0 && trackIndex < Mathf.Min(_strikeEffectOfTracks.Length, releaseEffectTweenOfTracks.Count)) {
                if (isHoldingOfTracks[trackIndex]) {
                    
                    if (releaseEffectTweenOfTracks[trackIndex] == null || !releaseEffectTweenOfTracks[trackIndex].IsActive()) {
                        releaseEffectTweenOfTracks[trackIndex] = GetReleaseEffectTweenOfTrack(trackIndex);
                    }

                    if (releaseEffectTweenOfTracks[trackIndex] != null && releaseEffectTweenOfTracks[trackIndex].IsActive()) {
                        releaseEffectTweenOfTracks[trackIndex].Restart();
                    }

                    isHoldingOfTracks[trackIndex] = false;
                }
            }
        }


        protected Tween GetReleaseEffectTweenOfTrack (int trackIndex) {

            if (trackIndex >= 0 && trackIndex < _strikeEffectOfTracks.Length) {
                GameObject strikeEffect = _strikeEffectOfTracks[trackIndex];
                if (strikeEffect) {
                    SpriteRenderer spriteRenderer = strikeEffect.GetComponent<SpriteRenderer>();
                    if (spriteRenderer) {
                        return spriteRenderer.DOFade(0f, _releaseEffectDuration)
                            .SetEase(_releaseEffectEase)
                            .OnComplete(() => {
                                if (strikeEffect) {
                                    strikeEffect.SetActive(false);
                                }
                            })
                            .SetAutoKill(true)
                            .Pause();
                    }
                }
            }

            return null;
        }

    }

}