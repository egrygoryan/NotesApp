using Microsoft.AspNetCore.Components;

namespace Notes.Pages;

public partial class NoteData
{
    [Inject]
    protected INoteService NoteService { get; set; }

    protected bool isAdd;
    protected bool isEdit;
    protected bool isFiltered;
    protected IEnumerable<NoteResponse> notes;
    protected string SearchString { get; set; } = string.Empty;
    protected AddNoteRequest addNoteRequest = new();
    protected UpdateNoteRequest updateNoteRequest = new();
    protected DeleteNoteRequest deleteNoteRequest = new();

    protected override async Task OnInitializedAsync()
    {
        await GetAllNotesAsync();
    }

    protected async Task GetAllNotesAsync()
    {
        notes = await NoteService.GetAllNotesAsync();
        isFiltered = false;
    }
    protected async Task AddNoteAsync()
    {
        await NoteService.AddNoteAsync(addNoteRequest);
        HideAddDialog();
        await GetAllNotesAsync();
    }

    protected async Task UpdateNoteAsync()
    {
        await NoteService.UpdateNoteAsync(updateNoteRequest);
        HideEditDialog();
        await GetAllNotesAsync();
    }

    protected async Task FilterNotesAsync()
    {
        isFiltered = true;
        notes = await NoteService.FindNotesAsync(SearchString);
        SearchString = string.Empty;
    }

    protected async Task DeleteNoteAsync(NoteResponse note)
    {
        deleteNoteRequest.Id = note.Id;
        await NoteService.DeleteNoteAsync(deleteNoteRequest);
        await GetAllNotesAsync();
    }
    protected void ShowAddDialog() => isAdd = true;
    protected void HideAddDialog()
    {
        addNoteRequest.Title = string.Empty;
        addNoteRequest.Text = string.Empty;
        isAdd = false;
    }
    protected void ShowEditDialog(NoteResponse note)
    {
        updateNoteRequest.MyUidAsString = note.Id.ToString();
        updateNoteRequest.Title = note.Title;
        updateNoteRequest.Text = note.Text;
        isEdit = true;
    }
    protected void HideEditDialog()
    {
        updateNoteRequest.MyUidAsString = string.Empty;
        updateNoteRequest.Title = string.Empty;
        updateNoteRequest.Text = string.Empty;
        isEdit = false;
    }
}
