using Moq;
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
public class NotesServiceUnitTests
{
    private readonly Mock<INoteRepository> _mockRepo;
    //Arrange - Act - Assert principle for tests
    public NotesServiceUnitTests() => _mockRepo = new Mock<INoteRepository>();

    [Fact]
    public async Task GetAllNotesAsync_ShouldBeNotEmptyAndReturnsNoteResponse()
    {
        _mockRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Note>
            {
                new Note
                {
                    Id = new Guid("42CF75BC-D0C9-4C07-FFFB-2783FE9F4E77"),
                    Title = "First",
                    Text = "First text",
                    CreationDate = DateTime.UtcNow
                },
                new Note
                {
                    Id = new Guid("529975BC-D1F9-4C07-AAFB-278CDE9F4E66"),
                    Title = "Second",
                    Text = "Second text",
                    CreationDate = DateTime.UtcNow
                }
            });

        var service = new NoteService(_mockRepo.Object);
        var notes = await service.GetAllNotesAsync();

        Assert.NotEmpty(notes);
        Assert.Equal(2, notes.ToList().Count);
        Assert.Equal(typeof(NoteResponse), notes.First().GetType());
    }

    [Fact]
    public async Task AddNoteAsync_ReposAddAsyncCalled()
    {
        var noteRequest = new AddNoteRequest() { Title = It.IsAny<string>(), Text = "Text" };

        var service = new NoteService(_mockRepo.Object);

        await service.AddNoteAsync(noteRequest);

        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Note>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task AddNoteAsync_ThrowsArgumentException_IfTextIsNotProvided(string title)
    {
        var noteRequest = new AddNoteRequest() { Title = "Title", Text = title };
        var expectedMessage = "Text field cannot be null or empty";
        var service = new NoteService(_mockRepo.Object);

        Func<Task> action = async () => await service.AddNoteAsync(noteRequest);

        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task UpdateNoteAsync_ReposUpdateAsyncCalled()
    {
        _mockRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Note
            {
                Id = It.IsAny<Guid>(),
                Title = It.IsAny<string>(),
                Text = It.IsAny<string>()
            });

        var noteRequest = new UpdateNoteRequest() { Id = It.IsAny<Guid>(), Title = It.IsAny<string>(), Text = "Example" };

        var service = new NoteService(_mockRepo.Object);

        await service.UpdateNoteAsync(noteRequest);
        _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<Note>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task UpdateNoteAsync_ThrowsArgumentException_IfTextIsNotProvided(string text)
    {
        var expectedMessage = "Text field cannot be null or empty";
        _mockRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new Note
           {
               Id = It.IsAny<Guid>(),
               Title = It.IsAny<string>(),
               Text = It.IsAny<string>()
           });

        var noteRequest = new UpdateNoteRequest() { Id = It.IsAny<Guid>(), Title = It.IsAny<string>(), Text = text };

        var service = new NoteService(_mockRepo.Object);

        Func<Task> action = async () => await service.UpdateNoteAsync(noteRequest);

        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("first", 1)]
    [InlineData("RETURN", 2)]
    [InlineData("example", 3)]
    public async Task FindNoteAsync_ReturnsMatchedTitleOrText(string wordToFind, int matches)
    {
        _mockRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Note>
            {
                new Note
                {
                    Id = new Guid("42CF75BC-D0C9-4C07-FFFB-2783FE9F4E77"),
                    Title = "First title. return.",
                    Text = "Test EXAMPLE",
                    CreationDate = DateTime.UtcNow
                },
                new Note
                {
                    Id = new Guid("529975BC-D1F9-4C07-AAFB-278CDE9F4E66"),
                    Title = "Test example",
                    Text = "Second text to return, where the comma is presented ",
                    CreationDate = DateTime.UtcNow
                },
                new Note
                {
                    Id = new Guid("111111FF-C5E2-4C07-DDDD-278CDE9F4784"),
                    Title = "Third example",
                    Text = "Third Example",
                    CreationDate = DateTime.UtcNow
                }
            });

        var service = new NoteService(_mockRepo.Object);

        var resultList = await service.FindNotesAsync(wordToFind);

        Assert.Equal(matches, resultList.ToList().Count);
        Assert.Equal(typeof(NoteResponse), resultList.First().GetType());
    }

    [Fact]
    public async Task FindNoteAsync_ReturnsEmptyCollection_WhenThereWereNoMatch()
    {
        string wordToFind = "TEST";
        _mockRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Note>
            {
                new Note
                {
                    Id = new Guid("42CF75BC-D0C9-4C07-FFFB-2783FE9F4E77"),
                    Title = "First title. return.",
                    Text = "EXAMPLE",
                    CreationDate = DateTime.UtcNow
                }
            });

        var service = new NoteService(_mockRepo.Object);

        var resultList = await service.FindNotesAsync(wordToFind);
        Assert.Empty(resultList);
    }
}