using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DoubleHeat.Serialization;

namespace ProjectComet.Levels {

    public static class BestRecordsManager {

        static readonly string _BEST_RECORDS_DATA_PATH = Application.persistentDataPath + "/best_records";


        static Dictionary<LevelIndicator, IBestLevelResultData> _loadedBestRecords = new Dictionary<LevelIndicator, IBestLevelResultData>();



        public static void SaveToBestRecord (int stageNumber, LevelDifficulty difficulty, IBestLevelResultData levelResultData) {
            List<IBestLevelResultData> resultDataList = new List<IBestLevelResultData>();

            IBestLevelResultData currentBestRecord = GetBestRecord(stageNumber, difficulty);
            if (currentBestRecord != null) {
                resultDataList.Add(currentBestRecord);
            }
            if (levelResultData != null) {
                resultDataList.Add(levelResultData);
            }

            IBestLevelResultData resultBestRecord = LevelResultData.GenerateNewForBestResult(resultDataList);

            _loadedBestRecords[new LevelIndicator(stageNumber, difficulty)] = resultBestRecord;
            SaveAsBestRecord(stageNumber, difficulty, resultBestRecord);
        }

        public static IBestLevelResultData GetBestRecord (int stageNumber, LevelDifficulty difficulty) {
            return GetBestRecord(new LevelIndicator(stageNumber, difficulty));
        }

        public static bool HasLevelCleared (int stageNumber, LevelDifficulty difficulty) {
            return HasLevelCleared(new LevelIndicator(stageNumber, difficulty));
        }

        public static bool IsHardModeUnlocked (int stageNumber) {
            return HasLevelCleared(stageNumber, LevelDifficulty.Normal);
        }

        public static bool IsStageUnlocked (int stageNumber) {
            int prevStageNumber = stageNumber - 1;
            if (prevStageNumber >= 1) {
                if (!HasLevelCleared(prevStageNumber, LevelDifficulty.Normal)) {
                    return false;
                }
            }
            return true;
        }


        static IBestLevelResultData GetBestRecord (LevelIndicator levelIndicator) {
            IBestLevelResultData result = null;
            
            if (!_loadedBestRecords.TryGetValue(levelIndicator, out result)) {
                result = LoadBestRecord(levelIndicator.stageNumber, levelIndicator.difficulty);
                _loadedBestRecords.Add(levelIndicator, result);
            }

            return result;
        }

        static bool HasLevelCleared (LevelIndicator levelIndicator) {
            var record = GetBestRecord(levelIndicator);
            if (record != null) {
                return record.IsLevelCleared;
            }
            return false;
        }


        static void SaveAsBestRecord (int stageNumber, LevelDifficulty difficulty, IBestLevelResultData bestRecord) {
            string recPath = GetBestRecordDataPath(stageNumber, difficulty);

            if (SerializationManager.Save(recPath, bestRecord)) {
                Debug.Log("Best record of level has saved.");
            }
            else {
                Debug.LogError("Failed to save best record of level!");
            }
        }

        static IBestLevelResultData LoadBestRecord (int stageNumber, LevelDifficulty difficulty) {
            string recPath = GetBestRecordDataPath(stageNumber, difficulty);

            if (File.Exists(recPath)) {
                object data = SerializationManager.Load(recPath);

                if (data is IBestLevelResultData) {
                    return (IBestLevelResultData) data;
                }
            }
            return null;
        }

        static string GetBestRecordDataPath (int stageNumber, LevelDifficulty difficulty) {
            return $"{ _BEST_RECORDS_DATA_PATH }/stage_{ stageNumber.ToString("00") }_{difficulty}.bestrec";
        }


        struct LevelIndicator {
            public int stageNumber;
            public LevelDifficulty difficulty;

            public LevelIndicator (int stageNumber, LevelDifficulty difficulty) {
                this.stageNumber = stageNumber;
                this.difficulty = difficulty;
            }

            public static bool operator == (LevelIndicator a, LevelIndicator b) {
                return a.Equals(b);
            }

            public static bool operator != (LevelIndicator a, LevelIndicator b) {
                return !a.Equals(b);
            }

            public override bool Equals (object obj) {
                return base.Equals(obj);
            }
            public override int GetHashCode() {
                return base.GetHashCode();
            }
        }

    }

}