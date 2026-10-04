using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Tests.TestHelpers;

namespace TodoApi.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="TodosController"/>.
/// Most endpoints (Add/Remove/FindAsync/SaveChangesAsync) are covered using a
/// mocked <see cref="TodoDbContext"/> and <see cref="DbSet{TodoItem}"/> via
/// Moq. The endpoints that rely on LINQ query translation
/// (OrderBy/Select/ToListAsync) can't be satisfied by a mocked DbSet without
/// a real provider, so those use the EF Core InMemory provider to back a
/// real <see cref="TodoDbContext"/> instead.
/// </summary>
public class TodosControllerTests
{
    private static (Mock<TodoDbContext> Context, Mock<DbSet<TodoItem>> Set) CreateMockContext(
        TodoItem? findAsyncResult = null)
    {
        var options = new DbContextOptions<TodoDbContext>();
        var mockContext = new Mock<TodoDbContext>(options);
        var mockSet = MockDbSetFactory.Create(findAsyncResult);

        mockContext.Setup(c => c.Todos).Returns(mockSet.Object);
        mockContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return (mockContext, mockSet);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithTodo_WhenFound()
    {
        var todo = new TodoItem
        {
            Id = 1,
            Title = "Buy milk",
            IsDone = false,
            CreatedAt = DateTime.UtcNow
        };
        var (context, _) = CreateMockContext(findAsyncResult: todo);
        var controller = new TodosController(context.Object);

        var result = await controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<TodoResponseDto>(okResult.Value);
        Assert.Equal(todo.Id, dto.Id);
        Assert.Equal(todo.Title, dto.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var (context, _) = CreateMockContext(findAsyncResult: null);
        var controller = new TodosController(context.Object);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_AndPersistsTodo_WhenValid()
    {
        var (context, set) = CreateMockContext();
        var controller = new TodosController(context.Object);

        var request = new CreateTodoDto
        {
            Title = "  Write tests  ",
            Description = "Cover the controller",
            DueDate = null
        };

        var result = await controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<TodoResponseDto>(created.Value);
        Assert.Equal("Write tests", dto.Title); // trimmed
        Assert.False(dto.IsDone);

        set.Verify(
            s => s.Add(It.Is<TodoItem>(t => t.Title == "Write tests")),
            Times.Once);
        context.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsValidationProblem_AndDoesNotPersist_WhenModelStateInvalid()
    {
        var (context, set) = CreateMockContext();
        var controller = new TodosController(context.Object);
        controller.ModelState.AddModelError("Title", "The Title field is required.");

        var result = await controller.Create(new CreateTodoDto { Title = "" });

        Assert.IsType<ObjectResult>(result.Result); // ValidationProblem -> 400 ObjectResult
        set.Verify(s => s.Add(It.IsAny<TodoItem>()), Times.Never);
        context.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_AndRemovesTodo_WhenFound()
    {
        var todo = new TodoItem { Id = 5, Title = "Old task" };
        var (context, set) = CreateMockContext(findAsyncResult: todo);
        var controller = new TodosController(context.Object);

        var result = await controller.Delete(5);

        Assert.IsType<NoContentResult>(result);
        set.Verify(s => s.Remove(todo), Times.Once);
        context.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_AndDoesNotSave_WhenMissing()
    {
        var (context, set) = CreateMockContext(findAsyncResult: null);
        var controller = new TodosController(context.Object);

        var result = await controller.Delete(123);

        Assert.IsType<NotFoundResult>(result);
        set.Verify(s => s.Remove(It.IsAny<TodoItem>()), Times.Never);
        context.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoTodosExist()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var controller = new TodosController(db);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var todos = Assert.IsAssignableFrom<IEnumerable<TodoResponseDto>>(okResult.Value);
        Assert.Empty(todos);
    }

    [Fact]
    public async Task GetAll_ReturnsTodos_OrderedByCreatedAt()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var now = DateTime.UtcNow;

        db.Todos.AddRange(
            new TodoItem { Title = "Second", CreatedAt = now.AddMinutes(1) },
            new TodoItem { Title = "First", CreatedAt = now }
        );
        await db.SaveChangesAsync();

        var controller = new TodosController(db);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var todos = Assert.IsAssignableFrom<IEnumerable<TodoResponseDto>>(okResult.Value).ToList();

        Assert.Equal(2, todos.Count);
        Assert.Equal("First", todos[0].Title);
        Assert.Equal("Second", todos[1].Title);
    }
}

