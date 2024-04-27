using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectComet.Levels {

    [Serializable]
    public class LevelResultData : IBestLevelResultData {

        public LevelResultRank ResultRank { get; protected set; }
        public bool IsLevelCleared { get; protected set; }

        public int Score { get; protected set; }
        public int MaxCombo { get; protected set; }
        public int PerfectCount { get; protected set; }
        public int GoodCount { get; protected set; }
        public int MissCount { get; protected set; }
        public int SafeCount { get; protected set; }
        public int FailCount { get; protected set; }

        // Achievements
        public bool HasAchievedClear { get; protected set; }
        public bool HasAchievedFullCombo { get; protected set; }
        public bool HasAchievedPerfect { get; protected set; }

        public LevelResultData () {}

        public LevelResultData (LevelResultRank rank, bool isCleared, int score, int maxCombo, int perfectCount, int goodCount, int missCount, int safeCount, int failCount) {
            ResultRank = rank;
            IsLevelCleared = isCleared;
            Score = score;
            MaxCombo = maxCombo;
            PerfectCount = perfectCount;
            GoodCount = goodCount;
            MissCount = missCount;
            SafeCount = safeCount;
            FailCount = failCount;

            HasAchievedClear = IsLevelCleared;
            HasAchievedFullCombo = (MissCount == 0 && FailCount == 0);
            HasAchievedPerfect = (HasAchievedFullCombo && GoodCount == 0);
        }


        public static IBestLevelResultData GenerateNewForBestResult (IEnumerable<IBestLevelResultData> results) {
            
            return new LevelResultData{
                ResultRank           = (LevelResultRank) results.Max(result => (byte) result.ResultRank),
                IsLevelCleared       = results.Any(result => result.IsLevelCleared),
                Score                = results.Max(result => result.Score),
                MaxCombo             = results.Max(result => result.MaxCombo),
                
                // Achievements
                HasAchievedClear     = results.Any(result => result.HasAchievedClear),
                HasAchievedFullCombo = results.Any(result => result.HasAchievedFullCombo),
                HasAchievedPerfect   = results.Any(result => result.HasAchievedPerfect)
            };

        }

        public override string ToString() {
            return $"Rank: {ResultRank}, IsCleared: {IsLevelCleared}, Score: {Score}, MaxCombo: {MaxCombo}, Clear: {HasAchievedClear}, FullCombo: {HasAchievedFullCombo}, AllPerfect: {HasAchievedPerfect}, Perfect: {PerfectCount}, Good: {GoodCount}, Miss: {MissCount}, Safe: {SafeCount}, Fail: {FailCount}";
        }

    }

}