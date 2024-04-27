using System;
using UnityEngine;
using DoubleHeat.Utilities;

namespace ProjectComet.Levels {

    public interface ILevelResultDataHandler : IMonoBehaviourAttached {

        void InitWithLevelResultData (LevelResultData levelResultData, int bestScore);

    }

}