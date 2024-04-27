using System;
using UnityEngine;

namespace ProjectComet.UIDisplay {

    public abstract class NumberDisplayController : MonoBehaviour {

        protected int? currentNumber { get; set; } = null;


        public virtual void SetCurrentNumber (int number) {
            if (currentNumber != number) {
                currentNumber = number;
                OnCurrentNumberChanged();
            }
        }

        protected abstract void OnCurrentNumberChanged ();


    }

}