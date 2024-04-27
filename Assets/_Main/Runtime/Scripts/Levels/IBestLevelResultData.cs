using System;
using System.Collections.Generic;

namespace ProjectComet.Levels {
    
    public interface IBestLevelResultData {
        LevelResultRank ResultRank { get; }
        bool IsLevelCleared { get; }
        int Score { get; }
        int MaxCombo { get; }

        bool HasAchievedClear { get; }
        bool HasAchievedFullCombo { get; }
        bool HasAchievedPerfect { get; }

    }
}