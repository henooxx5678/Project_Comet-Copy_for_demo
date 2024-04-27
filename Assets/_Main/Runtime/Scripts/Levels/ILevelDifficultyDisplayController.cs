using System;
using UnityEngine;
using DoubleHeat.Utilities;

namespace ProjectComet.Levels {
    public interface ILevelDifficultyDisplayController : IMonoBehaviourAttached {

        void SetSelected (LevelDifficulty difficulty);

    }
}