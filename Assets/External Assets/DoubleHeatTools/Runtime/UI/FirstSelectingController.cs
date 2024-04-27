using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoubleHeat.UI {
    public class FirstSelectingController : MonoBehaviour {

        [SerializeField] Timing _timing = Timing.OnEnable;
        [SerializeField] GameObject _firstSelectedByEventSystem;


        protected virtual void Start () {
            if (_timing == Timing.Start) {
                SelectTarget();
            }
        }

        protected virtual void OnEnable () {
            if (_timing == Timing.OnEnable) {
                SelectTarget();
            }
        }

        public void SelectTarget () {
            if (_firstSelectedByEventSystem && EventSystem.current) {
                EventSystem.current.SetSelectedGameObject(_firstSelectedByEventSystem);
            }
        }


        enum Timing {
            Start,
            OnEnable
        }

    }
}