namespace Notes.Domain.Services;

public interface INoteService
{
    Task<Guid> AddNoteAsync(AddNoteRequest request);
    Task<IEnumerable<NoteResponse>> GetAllNotesAsync();
    Task UpdateNoteAsync(UpdateNoteRequest request);
    Task<IEnumerable<NoteResponse>> FindNotesAsync(string word);
    Task DeleteNoteAsync(DeleteNoteRequest request);
}