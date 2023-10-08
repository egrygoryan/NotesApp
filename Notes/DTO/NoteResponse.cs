namespace Notes.DTO;

public record NoteResponse(
    Guid Id,
    string Title,
    string Text,
    DateTime CreationDate
);