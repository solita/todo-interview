using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    // virtual so it can be overridden by Moq in unit tests
    public virtual DbSet<TodoItem> Todos => Set<TodoItem>();
}
