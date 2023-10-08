namespace Notes.Data.Repositories;

public class CachedNoteRepository : INoteRepository
{
    //Key for our cache when all notes are needed
    private const string AllNotesCacheKey = "GET_ALL_NOTES";
    private readonly INoteRepository _repo;
    private readonly IMemoryCache _cache;
    public CachedNoteRepository(INoteRepository repo, IMemoryCache memoryCache) =>
        (_repo, _cache) = (repo, memoryCache);

    public async Task AddAsync(Note note)
    {
        //clear cache when add is triggered
        _cache.Remove(AllNotesCacheKey);
        await _repo.AddAsync(note);
    }

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        //if cache contains data - return it
        if (_cache.TryGetValue(AllNotesCacheKey, out List<Note> cachedNotes))
        {
            return cachedNotes;
        }

        //if no data in cache, we call db for retrieving data
        var notes = await _repo.GetAllAsync();

        //check if there are any notes
        //if it contains -fill in cache
        if (notes.Any())
        {
            _cache.Set(AllNotesCacheKey, notes);
        }

        return notes;
    }

    public async Task<Note> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);

    public async Task UpdateAsync(Note note)
    {
        //cache is invalid when the update event is triggered
        _cache.Remove(AllNotesCacheKey);

        await _repo.UpdateAsync(note);
    }

    public async Task DeleteAsync(Note note)
    {
        //cache is invalid when the delete event is triggered
        _cache.Remove(AllNotesCacheKey);

        await _repo.DeleteAsync(note);
    }
}
