using System;
using System.Collections;
using UnityEngine;
using DoubleHeat.UI;
using ProjectComet.Levels;

namespace ProjectComet.UIDisplay {
    
    public class LevelResultRankDisplayController : MonoBehaviour {
        

        // [SerializeField] ResultRankShowingObjectInfo[] _resultRankShowingObjectInfos;
        [SerializeField] Animator _controllingAnimator;

        LevelResultRank _currentShowingRank;


        public void ShowRank (LevelResultRank rank) {

            _currentShowingRank = rank;

            if (_controllingAnimator) {
                _controllingAnimator.gameObject.SetActive(false);
                _controllingAnimator.gameObject.SetActive(true);
            }

        }

        public void UpdateRankShowingDisplay () {
            if (_controllingAnimator && _currentShowingRank != LevelResultRank.None) {
                _controllingAnimator.SetTrigger(_currentShowingRank.ToString());
            }
        }


  
        // [Serializable]
        // protected class ResultRankShowingObjectInfo {
        //     [SerializeField] LevelResultRank _rank;
        //     public LevelResultRank Rank => _rank;

        //     [SerializeField] GameObject _showingObject;
        //     public GameObject ShowingObject => _showingObject;
        // }

    }

}