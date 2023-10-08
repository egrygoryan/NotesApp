using Microsoft.EntityFrameworkCore;
using Notes.Data.Context;
using Notes.Data.Entities;
using Notes.Data.Repositories;
using Notes.Domain.Services;
using Notes.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace NotesTest;
public class NotesServiceIntegrationTests : IDisposable
{
    private readonly NoteContext _context;
    private readonly NoteRepository _repository;

    public NotesServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<NoteContext>()
           .UseNpgsql("Host=localhost;Port=5432;Database=testdb;Username=testuser;Password=testpassword")
           .Options;

        _context = new NoteContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.Migrate();

        _repository = new NoteRepository(_context);
    }

    [Fact]
    public async Task AddNotesAsync_CreatesNoteAndNoteCanBeRetrievedFromDb()
    {
        var noteToAdd = new AddNoteRequest() { Title = $"Test Title", Text = $"Test Text" };

        var service = new NoteService(_repository);
        await service.AddNoteAsync(noteToAdd);

        var notesResponse = await service.GetAllNotesAsync();
        Assert.Single(notesResponse);
        Assert.Equal(noteToAdd.Title, notesResponse.First().Title);
    }

    [Fact]
    public async Task GetAllNotesAsync_RetrievesAllNotesFromDb()
    {
        _context.Notes.AddRange(Enumerable.Range(1, 3)
            .Select(x => new Note()
            {
                Id = Guid.NewGuid(),
                Title = $"Test Title {x}",
                Text = $"Test Text {x}",
                CreationDate = DateTime.UtcNow,
            }));
        _context.SaveChanges();

        var service = new NoteService(_repository);

        var notesResponse = await service.GetAllNotesAsync();
        Assert.Equal(3, notesResponse.ToList().Count);
        Assert.Contains("Test Text", notesResponse.First().Text);

    }

    [Theory]
    [InlineData("match")]
    public async Task FindNotesAsync_ReturnsMatchedNotesFromDb(string wordToFind)
    {
        var notesSet = new List<Note>()
        {
            new Note { Id = Guid.NewGuid(), Title = "Match value", Text = "Text", CreationDate = DateTime.UtcNow },
            new Note { Id = Guid.NewGuid(), Title = "Title", Text = "Value to match", CreationDate = DateTime.UtcNow },
            new Note { Id = Guid.NewGuid(), Title = "Title", Text = "Text", CreationDate = DateTime.UtcNow }
        };

        _context.Notes.AddRange(notesSet);
        _context.SaveChanges();

        var service = new NoteService(_repository);
        var resultNotes = await service.FindNotesAsync(wordToFind);

        Assert.Equal(2, resultNotes.ToList().Count);
    }

    [Fact]
    public async Task UpdatesNotesAsync_UpdatesProvidedNoteAndStoresItInDb()
    {
        var preSetNote = new Note { Id = Guid.NewGuid(), Title = "Title", Text = "Text", CreationDate = DateTime.UtcNow };
        _context.Notes.Add(preSetNote);
        _context.SaveChanges();

        var updatedTitle = "Updated Title";
        var updatedText = "Updated Text";
        var noteToUpdate = new UpdateNoteRequest() { Id = preSetNote.Id, Title = updatedTitle, Text = updatedText };

        var service = new NoteService(_repository);
        await service.UpdateNoteAsync(noteToUpdate);
        var updatedNote = await _repository.GetByIdAsync(noteToUpdate.Id);

        Assert.Equal(updatedTitle, updatedNote.Title);
        Assert.Equal(updatedText, updatedNote.Text);

    }

    [Fact]
    public async Task RepositoryGetByIdAsync_ThrowsApplicationExceptionIfIdIsNotMatched()
    {
        var preSetNote = new Note { Id = Guid.NewGuid(), Title = "Title", Text = "Text", CreationDate = DateTime.UtcNow };
        _context.Notes.Add(preSetNote);
        _context.SaveChanges();

        var newId = Guid.NewGuid();
        var message = $"No such note with ID: {newId}";


        Func<Task> action = async () => await _repository.GetByIdAsync(id: newId);

        var exception = await Assert.ThrowsAsync<ApplicationException>(action);
        Assert.Equal(message, exception.Message);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
