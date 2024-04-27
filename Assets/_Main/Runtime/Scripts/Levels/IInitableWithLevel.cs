using System;
using UnityEngine;
using DoubleHeat.Utilities;

namespace ProjectComet.Levels {

    public interface IInitableWithLevel : IMonoBehaviourAttached {
        void InitWithLevel (int stageNumber, LevelDifficulty difficulty);
    }

}
