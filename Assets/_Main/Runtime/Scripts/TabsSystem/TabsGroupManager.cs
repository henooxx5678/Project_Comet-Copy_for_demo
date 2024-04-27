using System;
using UnityEngine;

namespace ProjectComet.TabsSystem {
    
    public class TabsGroupManager : MonoBehaviour {
        

        [SerializeField] TabTargetInfo[] _tabTargetInfos;


        int _currentActiveTabIndex = 0;
        protected int CurrentActiveTabIndex {
            get => _currentActiveTabIndex;
            set {
                if (_currentActiveTabIndex != value) {
                    _currentActiveTabIndex = value;
                    UpdateStatus();
                }
            }
        }


        public void SwtichActiveTabToNext (bool loop) {
            int targetTabIndex = _currentActiveTabIndex + 1;

            if (loop) {
                targetTabIndex %= _tabTargetInfos.Length;
            }
            else {
                targetTabIndex = Math.Min(targetTabIndex, _tabTargetInfos.Length - 1);
            }

            CurrentActiveTabIndex = targetTabIndex;
        }

        public void SwitchActiveTabToPrevious (bool loop) {
            int targetTabIndex = _currentActiveTabIndex - 1;

            if (loop) {
                targetTabIndex = (targetTabIndex + _tabTargetInfos.Length) % _tabTargetInfos.Length;
            }
            else {
                targetTabIndex = Math.Max(targetTabIndex, 0);
            }

            CurrentActiveTabIndex = targetTabIndex;
        }

        public void SetActiveTabByName (string name) {
            int targetTabIndex = Array.FindIndex(_tabTargetInfos, info => info.Name == name);

            if (targetTabIndex >= 0) {
                CurrentActiveTabIndex = targetTabIndex;
            }
        }


        public void UpdateStatus () {
            for (int i = 0 ; i < _tabTargetInfos.Length ; i++) {
                _tabTargetInfos[i].TargetGameObject.SetActive(i == _currentActiveTabIndex);
            }
        }


        [Serializable]
        protected class TabTargetInfo {

            [SerializeField] string _name;
            public string Name => _name;
            
            [SerializeField] GameObject _targetGameObject;
            public GameObject TargetGameObject => _targetGameObject;
        }

    }

}