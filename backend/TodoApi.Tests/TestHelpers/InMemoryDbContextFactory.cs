using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Tests.TestHelpers;

/// <summary>
/// Creates isolated, real <see cref="TodoDbContext"/> instances backed by the
/// EF Core InMemory provider. Used for tests that need actual LINQ query
/// translation (OrderBy/Select/ToListAsync), which can't be exercised with a
/// plain Moq'd DbSet.
/// </summary>
public static class InMemoryDbContextFactory
{
    public static TodoDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TodoDbContext(options);
    }
}

