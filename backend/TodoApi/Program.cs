using Microsoft.EntityFrameworkCore;
using TodoApi.Configuration;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

const string frontendCorsPolicy = "FrontendDev";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core backed by the InMemory provider - no real database needed.
// Data resets every time the app restarts.
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodosDb"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed the in-memory database from a JSON file at startup.
app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(frontendCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
