using System.Text.Json;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Configuration;

public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the in-memory database from a JSON file at startup, if it is empty.
    /// </summary>
    public static void SeedDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "todos.seed.json");

        if (!File.Exists(seedPath) || db.Todos.Any())
        {
            return;
        }

        var json = File.ReadAllText(seedPath);
        var items = JsonSerializer.Deserialize<List<TodoItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items is { Count: > 0 })
        {
            db.Todos.AddRange(items);
            db.SaveChanges();
        }
    }
}

