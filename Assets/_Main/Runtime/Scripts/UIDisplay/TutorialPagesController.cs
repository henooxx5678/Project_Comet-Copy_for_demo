using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DoubleHeat.UI;

namespace ProjectComet.UIDisplay {

    public class TutorialPagesController : MonoBehaviour {

        [SerializeField] GameObject[] _pages;

        public UnityEvent onPageChanged;

        int _currentPageIndex = 0;
        public int CurrentPageIndex {
            get => _currentPageIndex;
            set {
                if (_currentPageIndex != value) {
                    _currentPageIndex = Math.Max(value, 0);
                    _currentPageIndex = Math.Min(_currentPageIndex, _pages.Length - 1);

                    for (int i = 0 ; i < _pages.Length ; i++) {
                        if (_pages[i]) {
                            _pages[i].SetActive(i == _currentPageIndex);
                        }
                    }

                    onPageChanged?.Invoke();
                }
            }
        }


        protected virtual void OnEnable () {
            CurrentPageIndex = 0;
        }


        public void SwitchPage (int step) {
            CurrentPageIndex += step;
        }

    }

}
