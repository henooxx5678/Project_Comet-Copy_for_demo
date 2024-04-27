using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectComet.OptionsHandling {

   
    public abstract class OptionsApplier : MonoBehaviour {
    
        protected virtual void OnEnable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.OptionsApplied += OnOptionsApplied;
            }

            ApplyCurrentOptionsData();
        }

        protected virtual void OnDisable () {
            if (OptionsHolder.current) {
                OptionsHolder.current.OptionsApplied -= OnOptionsApplied;
            }
        }


        protected void OnOptionsApplied (object sender, EventArgs args) {
            ApplyCurrentOptionsData();
        }

        protected void ApplyCurrentOptionsData () {
            if (OptionsHolder.current) {
                ApplyOptionsData(OptionsHolder.current.CurrentOptions);
            }
        }

        protected abstract void ApplyOptionsData (OptionsData data);

    }
}        