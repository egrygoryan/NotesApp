namespace Notes.Data.Context;

public class NoteContext : DbContext
{
    public DbSet<Note> Notes { get; set; }

    public NoteContext(DbContextOptions<NoteContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}