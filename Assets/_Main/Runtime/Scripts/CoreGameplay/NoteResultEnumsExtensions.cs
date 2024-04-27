namespace ProjectComet.CoreGameplay {

    public static class NoteResultEnumsExtensions {
        
        public static NoteResult ToNoteResult (this StrikeResult strikeResult) {
            switch (strikeResult) {
                case StrikeResult.Perfect: {
                    return NoteResult.Perfact;
                }
                case StrikeResult.Good: {
                    return NoteResult.Good;
                }
            }
            return NoteResult.None;
        }

        public static NoteResult ToNoteResult (this MonsterPassResult monsterPassResult) {
            switch (monsterPassResult) {
                case MonsterPassResult.Safe: {
                    return NoteResult.Safe;
                }
                case MonsterPassResult.Fail: {
                    return NoteResult.Fail;
                }
            }
            return NoteResult.None;
        }

        public static StrikeResult ToStrikeResult (this NoteResult noteResult) {
            switch (noteResult) {
                case NoteResult.Perfact: {
                    return StrikeResult.Perfect;
                }
                case NoteResult.Good: {
                    return StrikeResult.Good;
                }
            }
            return StrikeResult.None;
        }

        public static MonsterPassResult ToMonsterPassResult (this NoteResult noteResult) {
            switch (noteResult) {
                case NoteResult.Safe: {
                    return MonsterPassResult.Safe;
                }
                case NoteResult.Fail: {
                    return MonsterPassResult.Fail;
                }
            }
            return MonsterPassResult.None;
        }

    }

}