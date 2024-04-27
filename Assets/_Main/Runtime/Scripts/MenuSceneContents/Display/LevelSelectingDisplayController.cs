using System;
using UnityEngine;
using DG.Tweening;

namespace ProjectComet.MenuSceneContents.Display {

    public class LevelSelectingDisplayController : MonoBehaviour {

        [SerializeField] int _initStageNumber = 1;
        [SerializeField] Vector2 _planetsAnchoredPosIntervalVector = new Vector2(0f, 1f);
        [SerializeField] float _levelSwtichingDuration = 1f;
        [SerializeField] Ease _levelSwitchingAnimationEase;

        [Header("REFS")]
        [SerializeField] RectTransform _planetsViewRectTransform;


        Vector2 _planetsViewInitAnchoredPos;
        int _currentStageNumber = 1;

        Tween _currentPlanetsViewAnimTween;


        protected virtual void Awake () {
            if (_planetsViewRectTransform) {
                _planetsViewInitAnchoredPos = _planetsViewRectTransform.anchoredPosition;
            }
            else {
                Debug.LogWarning("\"_planetsViewRectTransform\" is empty!");
            }
        }


        public void UpdateStatus (int newStageNumber) {
            if (newStageNumber != _currentStageNumber) {
                _currentStageNumber = newStageNumber;

                if (_planetsViewRectTransform) {
                    if (_currentPlanetsViewAnimTween != null && _currentPlanetsViewAnimTween.IsActive()) {
                        _currentPlanetsViewAnimTween.Kill(false);
                    }

                    _currentPlanetsViewAnimTween = DOTween.To(
                        () => _planetsViewRectTransform.anchoredPosition, 
                        pos => _planetsViewRectTransform.anchoredPosition = pos, 
                        _planetsViewInitAnchoredPos + (Vector2) (_planetsViewRectTransform.localRotation * _planetsAnchoredPosIntervalVector * (_currentStageNumber - _initStageNumber)), 
                        _levelSwtichingDuration
                    )
                        .SetEase(_levelSwitchingAnimationEase);
                }
            }
        }


    }
}