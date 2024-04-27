using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectComet.UIDisplay {

    public class NumberDisplayControllerWithSprite : NumberDisplayController {

        [SerializeField] NumberSpritePair[] _numberSpritePairs;
        [SerializeField] NumberTextPair[] _numberTextPairs;
        
        [Header("Target Sprite Carriers")]
        [SerializeField] SpriteRenderer[] _targetSpriteRenderers;
        [SerializeField] Image[] _targetImages;

        [Header("Target Text Carriers")]
        [SerializeField] Text[] _targetUnityTextUIs;



        protected override void OnCurrentNumberChanged () {
            Sprite targetSprite = null;

            if (currentNumber != null) {
                targetSprite = NumberSpritePair.GetTargetSprite(_numberSpritePairs, (int) currentNumber);
            }

            if (_targetSpriteRenderers != null) {
                foreach (var spriteCarrier in _targetSpriteRenderers) {
                    if (spriteCarrier) {
                        spriteCarrier.sprite = targetSprite;
                    }
                }
            }

            if (_targetImages != null) {
                foreach (var spriteCarrier in _targetImages) {
                    if (spriteCarrier) {
                        spriteCarrier.sprite = targetSprite;
                    }
                }
            }
            
            string targetText = NumberTextPair.GetTargetText(_numberTextPairs, (int) currentNumber);
            if (_targetUnityTextUIs != null) {
                foreach (var unityTextUI in _targetUnityTextUIs) {
                    unityTextUI.text = targetText;
                }
            }
        }


        [Serializable]
        protected class NumberSpritePair {
            public int number;
            public Sprite sprite;

            public static Sprite GetTargetSprite (IEnumerable<NumberSpritePair> pairs, int number) {
                foreach (var pair in pairs) {
                    if (pair.number == number) {
                        return pair.sprite;
                    }
                }
                return null;
            }
        }

        [Serializable]
        protected class NumberTextPair {
            public int number;
            public string text;

            public static string GetTargetText (IEnumerable<NumberTextPair> pairs, int number) {
                foreach (var pair in pairs) {
                    if (pair.number == number) {
                        return pair.text;
                    }
                }
                return string.Empty;
            }
        }

    }

}