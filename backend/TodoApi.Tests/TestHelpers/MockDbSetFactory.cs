using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Models;

namespace TodoApi.Tests.TestHelpers;

/// <summary>
/// Helper for building a Moq-based <see cref="DbSet{TEntity}"/> for
/// <see cref="TodoItem"/> for tests that only need to exercise
/// Add / Remove / FindAsync (i.e. no LINQ query translation).
///
/// EF Core's <see cref="DbSet{TEntity}"/> members like Add, Remove and
/// FindAsync are declared virtual specifically so they can be mocked.
/// Queries that use LINQ operators (OrderBy/Select/ToListAsync) require a
/// real IQueryable provider, so those are tested against the EF Core
/// InMemory provider instead - see <see cref="InMemoryDbContextFactory"/>.
/// </summary>
public static class MockDbSetFactory
{
    public static Mock<DbSet<TodoItem>> Create(TodoItem? findAsyncResult = null)
    {
        var mockSet = new Mock<DbSet<TodoItem>>();

        mockSet
            .Setup(s => s.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync(findAsyncResult);

        return mockSet;
    }
}

