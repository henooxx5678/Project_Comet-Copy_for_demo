using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DoubleHeat.UI {
    public class SelectableByPointerEnter : MonoBehaviour, IPointerEnterHandler {

        Selectable _targetSelectable;
        public Selectable TargetSelectable => _targetSelectable ??= this.GetComponent<Selectable>();

        public bool Interactable => TargetSelectable ? TargetSelectable.interactable : false;


        public void OnPointerEnter (PointerEventData eventData) {
            
            if (Interactable) {
                if (EventSystem.current && this.gameObject) {
                    EventSystem.current.SetSelectedGameObject(this.gameObject);
                }
            }
            
        }

    }
}