namespace Notes.DTO;
public record AddNoteRequest
{
    public string Title { get; set; }
    [Required]
    public string Text { get; set; }
}