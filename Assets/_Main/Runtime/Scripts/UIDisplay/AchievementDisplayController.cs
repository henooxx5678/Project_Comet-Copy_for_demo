using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DoubleHeat.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {
    
    public class AchievementDisplayController : MonoBehaviour {
        
        // same GameObject
        [SerializeField] Animator _animator;

        bool _hasAchieved = false;
        public bool HasAchieved {
            get => _hasAchieved;
            set {
                _hasAchieved = value;
                this.gameObject.SetActive(false);
                this.gameObject.SetActive(true);
            }
        }

        protected virtual void OnEnable () {
            if (HasAchieved) {
                _animator.SetTrigger("Done");
            }
        }


    }

}