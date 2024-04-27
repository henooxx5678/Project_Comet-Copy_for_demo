using System;
using UnityEngine;

namespace ProjectComet.MenuSceneContents {
        
    public class MenuSceneManager : MonoBehaviour {

        [SerializeField] string _defaultStageNameAtStart;

        [SerializeField] StageInfo[] _stageInfos;


        public bool IsAnyStageActive => !string.IsNullOrEmpty(CurrentStageName);

        public string CurrentStageName {
            get {
                foreach (StageInfo info in _stageInfos) {
                    if (info.IsStageActive) {
                        return info.Name;
                    }
                }
                return string.Empty;
            }
        }


        protected virtual void Start () {
            if (!IsAnyStageActive) {
                SetCurrentStage(_defaultStageNameAtStart);
            }
        }


        public void SetCurrentStage (string stageName) {
            foreach (StageInfo info in _stageInfos) {
                if (info.Name == stageName) {
                    info.SetStageActive(true);
                }
                else {
                    info.SetStageActive(false);
                }
            }
        }

        
        [Serializable]
        public class StageInfo {
            [SerializeField] string _name;
            public string Name => _name;

            [SerializeField] GameObject _stageObject;

            public bool IsStageActive => _stageObject.activeSelf;

            public void SetStageActive (bool isActive) {
                _stageObject.SetActive(isActive);
            }
        }

    }

}