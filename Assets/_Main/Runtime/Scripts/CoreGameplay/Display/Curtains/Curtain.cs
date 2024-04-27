using System.Collections.Generic;
using UnityEngine;

namespace ProjectComet.CoreGameplay.Display.Curtains {

    
    public class Curtain : MonoBehaviour {
        
        static List<Curtain> _instances = new List<Curtain>();
        public static IEnumerable<Curtain> Instances => _instances.ToArray();


        [SerializeField] Color _gizmosDrawnColor;
        [SerializeField] Vector3 _gizmosDrawnSize;



        protected virtual void OnDrawGizmos () {
            Gizmos.color = _gizmosDrawnColor;
            Gizmos.DrawCube(transform.position, _gizmosDrawnSize);
        }


        
        protected virtual void OnEnable () {
            _instances.Add(this);
        }

        protected virtual void OnDisable () {
            _instances.Remove(this);
        }
    }
}