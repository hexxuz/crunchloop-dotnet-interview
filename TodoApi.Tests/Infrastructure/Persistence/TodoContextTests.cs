using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;

namespace TodoApi.Tests.Infrastructure.Persistence;

public class TodoContextTests
{
    private TodoContext CreateContext()
    {
        DbContextOptions<TodoContext> options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TodoContext(options);
    }

    #region Lists
    [Fact]
    public async Task CanCreateTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Item" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoList.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CanReadTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Item" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoList? saved = await context.TodoList.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Item", saved.Name);
    }

    [Fact]
    public async Task CanUpdateTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Item" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        list.Name = "Updated";
        await context.SaveChangesAsync(CancellationToken.None);

        TodoList? updated = await context.TodoList.FirstOrDefaultAsync();
        Assert.Equal("Updated", updated.Name);
    }

    [Fact]
    public async Task CanDeleteTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Item" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        context.TodoList.Remove(list);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoList.CountAsync();
        Assert.Equal(0, count);
    }
    #endregion

    #region Items
    [Fact]
    public async Task CanCreateTodoItem()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Item", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoItem.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CanReadTodoItem()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Item", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoItem? saved = await context.TodoItem.Include(i => i.List).FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Item", saved.Name);
        Assert.Equal("List", saved.List.Name);
    }

    [Fact]
    public async Task CanUpdateTodoItem()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Item", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        item.Name = "Updated";
        await context.SaveChangesAsync(CancellationToken.None);

        TodoItem? updated = await context.TodoItem.FirstOrDefaultAsync();
        Assert.Equal("Updated", updated.Name);
    }

    [Fact]
    public async Task CanDeleteTodoItem()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Item", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        context.TodoItem.Remove(item);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoItem.CountAsync();
        Assert.Equal(0, count);
    }

    #endregion
}
