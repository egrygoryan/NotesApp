namespace Notes.Configuration;

public static class WebScrapingConfigurationExtension
{
    public static IServiceCollection AddNoteServices(this IServiceCollection services, string connection) =>
        services
            .AddDbContext<NoteContext>(options =>
                options.UseNpgsql(connection))
            .AddScoped<INoteRepository, NoteRepository>()
            .Decorate<INoteRepository, CachedNoteRepository>()
            .AddScoped<INoteService, NoteService>();
}