using System;
using UnityEngine;

namespace ProjectComet.Infos {

    [CreateAssetMenu(fileName = "StageBasicInfo", menuName = "ScriptableObjects/StageBasicInfo")]
    public class StageBasicInfo : ScriptableObject {

        [SerializeField] string _name;
        public string Name => _name;

        [SerializeField] string _composer;
        public string Composer => _composer;

    }
}