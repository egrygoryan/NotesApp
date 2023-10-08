namespace Notes.Data.Repositories;

public interface INoteRepository
{
    Task AddAsync(Note note);
    Task<IEnumerable<Note>> GetAllAsync();
    Task<Note> GetByIdAsync(Guid id);
    Task UpdateAsync(Note note);
    Task DeleteAsync(Note note);
}
