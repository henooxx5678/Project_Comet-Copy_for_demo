using System;
using UnityEngine;

namespace DoubleHeat.Common {

    
    public class DestroyingHandler : MonoBehaviour {

        public event EventHandler<EventArgs> Destroyed;


        void OnDestroy () {
            Destroyed?.Invoke(this, EventArgs.Empty);
        }


        public void DestroySelf () {
            Destroy(this.gameObject);
        }

    }
}