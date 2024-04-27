using System;
using UnityEngine;
using DG.Tweening;


namespace ProjectComet.Common {

    public class TabInstructorMovingController : MonoBehaviour {

        [SerializeField] float _switchingDuration = 0.2f;
        [SerializeField] Ease _switchingEase;

        public RectTransform rectTransform => transform is RectTransform ? (RectTransform) transform : null;

        Tween _currentMovingTween = null;
        


        public void MoveHorizontally (RectTransform targetRectTransform) {
            Tween movingTween = this.rectTransform.DOAnchorPosX(targetRectTransform.anchoredPosition.x, _switchingDuration, false)
                .SetEase(_switchingEase)
                .SetUpdate(UpdateType.Normal, true);
            

            SetCurrentMovingTween(movingTween);
        }


        protected void SetCurrentMovingTween (Tween movingTween) {
            if (_currentMovingTween.IsActive()) {
                _currentMovingTween.Kill(false);
            }
            _currentMovingTween = movingTween;
        }

    }

}