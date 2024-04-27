namespace ProjectComet.CoreGameplay.Notes {
    
    public interface IInteractableNoteOnSheet : IOffsetTimeCarrier {
        Note.Type NoteType { get; }
        int[] ExistedOnTracksIndex { get; }
        int[] RemainedOnTracksIndex { get; }
    }

    
}