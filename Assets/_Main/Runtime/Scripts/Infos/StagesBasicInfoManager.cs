using System;
using UnityEngine;
using DoubleHeat.Common;

namespace ProjectComet.Infos {
    public class StagesBasicInfoManager : SingletonMonoBehaviour<StagesBasicInfoManager> {

        [SerializeField] StageBasicInfo[] _stageBasicInfos;


        public StageBasicInfo GetStageBasicInfoByStageNumber (int stageNumber) {
            int index = stageNumber - 1;
            if (index >= 0 && index < _stageBasicInfos.Length) {
                return _stageBasicInfos[index];
            }
            return null;
        }

    }
}