using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DoubleHeat.Utilities;

namespace ProjectComet.UIDisplay {
    
    [RequireComponent(typeof(RectTransform))]
    public class StayingInScrollRectController : MonoBehaviour {
        
        [SerializeField] float _minAnchorPosYForFirstElement = Mathf.NegativeInfinity;
        [SerializeField] float _maxAnchorPosYForFirstElement = Mathf.Infinity;
        [SerializeField] float _elementsInterval;

        RectTransform _rectTransform;
        public RectTransform SelfRectTransform => _rectTransform ? _rectTransform : _rectTransform = this.GetComponent<RectTransform>();

        
        public void StayInScrollRect (int elementIndex) {
            if (SelfRectTransform) {
                float minY = _minAnchorPosYForFirstElement + _elementsInterval * elementIndex;
                float maxY = _maxAnchorPosYForFirstElement + _elementsInterval * elementIndex;

                if (SelfRectTransform.anchoredPosition.y < minY) {
                    SelfRectTransform.anchoredPosition = SelfRectTransform.anchoredPosition.GetAfterSetY(minY);
                }
                if (SelfRectTransform.anchoredPosition.y > maxY) {
                    SelfRectTransform.anchoredPosition = SelfRectTransform.anchoredPosition.GetAfterSetY(maxY);
                }
            }
        }

    }

}