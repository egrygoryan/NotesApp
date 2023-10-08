namespace Notes.Data.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly NoteContext _ctx;

    public NoteRepository(NoteContext noteCtx) => _ctx = noteCtx;

    public async Task AddAsync(Note note)
    {
        await _ctx.Notes.AddAsync(note);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<Note>> GetAllAsync() => await Task.FromResult(_ctx.Notes.ToList());

    public async Task UpdateAsync(Note note)
    {
        _ctx.Entry(note).State = EntityState.Modified;
        await _ctx.SaveChangesAsync();
    }

    public async Task<Note> GetByIdAsync(Guid id)
    {
        var note = await _ctx.Notes.FirstOrDefaultAsync(x => x.Id == id);
        if (note is null)
        {
            throw new ApplicationException($"No such note with ID: {id}");
        }

        return note;
    }

    public async Task DeleteAsync(Note note)
    {
        _ctx.Notes.Remove(note);
        await _ctx.SaveChangesAsync();
    }
}
