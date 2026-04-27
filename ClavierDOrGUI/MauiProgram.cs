using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.EntityFrameworkCore;
using ClavierDOrGUI.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ClavierDOrGUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Configure SQLite DbContext
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "clavierdor.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Register database related services
        builder.Services.AddSingleton<CsvQuestionSeeder>();
        builder.Services.AddSingleton<DatabaseService>();

        // Register game services
        builder.Services.AddScoped<Services.ScoreService>();
        builder.Services.AddScoped<Services.GameService>();

        var app = builder.Build();

        // Initialize DB and seed synchronously on startup
        using (var scope = app.Services.CreateScope())
        {
            var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
            dbService.InitializeAsync().GetAwaiter().GetResult();
        }

        return app;
    }
}