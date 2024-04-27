using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DoubleHeat.UI;

namespace ProjectComet.Common.UnityUI {

    [RequireComponent(typeof(FirstSelectingController))]
    public class NavigationSelectorWhenNothingSelected : NavigationHandler {
        
        static List<NavigationSelectorWhenNothingSelected> _stackedList = new List<NavigationSelectorWhenNothingSelected>();


        FirstSelectingController _firstSelectingController;
        public FirstSelectingController TargetFirstSelectingController => _firstSelectingController ? _firstSelectingController : _firstSelectingController = this.GetComponent<FirstSelectingController>();



        protected override void OnEnable () {
            base.OnEnable();

            _stackedList.Add(this);
        }

        protected override void OnDisable () {
            base.OnDisable();

            _stackedList.Remove(this);
        }


        protected override void OnNavigate(Vector2 value) {
            if (_stackedList[_stackedList.Count - 1] == this) {
                base.OnNavigate(value);
                if (EventSystem.current && (!EventSystem.current.currentSelectedGameObject || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)) {
                    if (TargetFirstSelectingController) {
                        TargetFirstSelectingController.SelectTarget();
                    }
                }
            }
        }

    }

}