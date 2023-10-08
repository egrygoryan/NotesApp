namespace Notes.DTO;

public record DeleteNoteRequest
{
    public string MyUidAsString { get; set; }

    public Guid Id
    {
        get { return Guid.TryParse(MyUidAsString, out Guid g) ? g : default; }
        set { MyUidAsString = Convert.ToString(value); }
    }
}
