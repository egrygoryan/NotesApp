namespace Notes.Domain.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _repo;
    public NoteService(INoteRepository noteRepository) => _repo = noteRepository;

    public async Task<Guid> AddNoteAsync(AddNoteRequest request)
    {
        //check is the text is empty
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new ArgumentException("Text field cannot be null or empty");
        }

        var note = new Note
        {
            Id = new Guid(),
            Text = request.Text,
            Title = request.Title ?? "No Title",
            CreationDate = DateTime.UtcNow
        };

        await _repo.AddAsync(note);

        return note.Id;
    }

    public async Task<IEnumerable<NoteResponse>> GetAllNotesAsync()
    {
        var notes = await _repo.GetAllAsync();

        //transform fetched data to response format
        var noteResponse = notes
            .Select(x => new NoteResponse(
                Id: x.Id,
                Title: x.Title,
                Text: x.Text,
                CreationDate: x.CreationDate.ToLocalTime()))
            .OrderBy(x => x.CreationDate)
            .ToList();

        return noteResponse;
    }

    public async Task<IEnumerable<NoteResponse>> FindNotesAsync(string wordToFind)
    {
        //retrieve notes
        var notes = await _repo.GetAllAsync();

        //check all notes for the specified word
        var resultNoteList = notes.Aggregate(new List<Note>(), (similarNotes, note) =>
        {
            var title = note.Title.Split(new[] { ' ', ',', '.' })
                .Any(x => x.Equals(wordToFind, StringComparison.InvariantCultureIgnoreCase));

            var text = note.Text.Split(new[] { ' ', ',', '.' })
                .Any(x => x.Equals(wordToFind, StringComparison.InvariantCultureIgnoreCase));
            if (title || text == true)
            {
                similarNotes.Add(note);
            }

            return similarNotes;
        });

        //empty enumerable returned if there were no matches
        if (!resultNoteList.Any())
        {
            return Enumerable.Empty<NoteResponse>();
        }

        //map note to responseType
        var response = resultNoteList
            .Select(x => new NoteResponse(
                Id: x.Id,
                Title: x.Title,
                Text: x.Text,
                CreationDate: x.CreationDate.ToLocalTime()))
            .ToList();

        return response;
    }

    public async Task UpdateNoteAsync(UpdateNoteRequest request)
    {
        //check is the text is empty
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new ArgumentException("Text field cannot be null or empty");
        }

        //find requested note
        var noteToUpdate = await _repo.GetByIdAsync(request.Id);

        //update its properties 
        noteToUpdate.Text = request.Text;
        noteToUpdate.Title = request.Title;

        //call db for update
        await _repo.UpdateAsync(noteToUpdate);
    }

    public async Task DeleteNoteAsync(DeleteNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MyUidAsString))
        {
            throw new ArgumentException("Guid cannot be null or empty");
        }

        //find a note
        var note = await _repo.GetByIdAsync(request.Id);
        //remove note from repo
        await _repo.DeleteAsync(note);
    }
}