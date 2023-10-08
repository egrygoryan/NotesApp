namespace Notes.DTO;
public record UpdateNoteRequest
{
    //blazor input component doesn't work with GUID type
    //https://github.com/dotnet/aspnetcore/issues/9939
    public string MyUidAsString { get; set; }

    public Guid Id
    {
        get { return Guid.TryParse(MyUidAsString, out Guid g) ? g : default; }
        set { MyUidAsString = Convert.ToString(value); }
    }
    public string Title { get; set; }
    [Required]
    public string Text { get; set; }
}