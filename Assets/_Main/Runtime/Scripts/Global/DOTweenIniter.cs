using UnityEngine;
using DG.Tweening;

namespace ProjectComet.Global { 
    
    public class DOTweenIniter : MonoBehaviour {
        
        protected virtual void Awake () {
            DOTween.Init();
        }

    }

}