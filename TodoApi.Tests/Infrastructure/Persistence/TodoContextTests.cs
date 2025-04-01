using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;

namespace TodoApi.Tests.Infrastructure.Persistence;

public class TodoContextTests
{
    private TodoContext CreateContext()
    {
        DbContextOptions<TodoContext> options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        return new TodoContext(options);
    }

    [Fact]
    public async Task CanSaveTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Test" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        Assert.Single(context.TodoList.ToList());
    }

    [Fact]
    public async Task CanSaveTodoItemWithList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Test", TodoItems = new List<TodoItem>() };
        TodoItem item = new TodoItem { Name = "Item" };
        list.TodoItems.Add(item);

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoList? savedList = context.TodoList.Include(l => l.TodoItems).FirstOrDefault();
        Assert.NotNull(savedList);
        Assert.Single(savedList.TodoItems);
    }

    [Fact]
    public async Task CanQueryItemWithList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Item", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoItem? loaded = context.TodoItem.Include(i => i.List).FirstOrDefault();
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.List);
    }

    [Fact]
    public async Task CanCreateReadUpdateDeleteTodoList()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "Item" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoList? saved = await context.TodoList.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Item", saved.Name);

        saved.Name = "Updated";
        await context.SaveChangesAsync(CancellationToken.None);

        TodoList? updated = await context.TodoList.FirstOrDefaultAsync();
        Assert.Equal("Updated", updated.Name);

        context.TodoList.Remove(updated);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoList.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CanCreateReadUpdateDeleteTodoItem()
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

        saved.Name = "Updated";
        await context.SaveChangesAsync(CancellationToken.None);

        TodoItem? updated = await context.TodoItem.FirstOrDefaultAsync();
        Assert.Equal("Updated", updated.Name);

        context.TodoItem.Remove(updated);
        await context.SaveChangesAsync(CancellationToken.None);

        int count = await context.TodoItem.CountAsync();
        Assert.Equal(0, count);
    }
}
