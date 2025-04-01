using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Application.TodoItems.Queries;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Application.TodoItems.Queries;

public class GetTodoItemsQueryHandlerTests
{
    // ToDo: Avoid repeating create context on every tests
    private TodoContext CreateContext()
    {
        DbContextOptions<TodoContext> options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TodoContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenListDoesNotExist()
    {
        TodoContext context = CreateContext();
        GetTodoItemsQueryHandler handler = new GetTodoItemsQueryHandler(new TodoListsHelper(context));

        GetTodoItemsQuery query = new GetTodoItemsQuery { ListId = 1 };
        ActionResult result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenListIsEmpty()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List", TodoItems = new List<TodoItem>() };

        context.TodoList.Add(list);
        await context.SaveChangesAsync();

        GetTodoItemsQueryHandler handler = new GetTodoItemsQueryHandler(new TodoListsHelper(context));
        GetTodoItemsQuery query = new GetTodoItemsQuery { ListId = list.Id };

        ActionResult result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnItems_WhenListHasItems()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item1 = new TodoItem { Name = "Test 1", List = list };
        TodoItem item2 = new TodoItem { Name = "Test 2", List = list };

        context.TodoList.Add(list);
        context.TodoItem.AddRange(item1, item2);
        await context.SaveChangesAsync();

        GetTodoItemsQueryHandler handler = new GetTodoItemsQueryHandler(new TodoListsHelper(context));
        GetTodoItemsQuery query = new GetTodoItemsQuery { ListId = list.Id };

        ActionResult result = await handler.Handle(query, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<TodoItemDTO> items = Assert.IsType<List<TodoItemDTO>>(ok.Value);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, x => x.Name == "Test 1");
        Assert.Contains(items, x => x.Name == "Test 2");
    }

    [Fact]
    public async Task Handle_ShouldReturn500_OnException()
    {
        TodoContext context = CreateContext();
        context.Dispose();

        GetTodoItemsQueryHandler handler = new GetTodoItemsQueryHandler(new TodoListsHelper(context));
        GetTodoItemsQuery query = new GetTodoItemsQuery { ListId = 1 };

        ActionResult result = await handler.Handle(query, CancellationToken.None);

        StatusCodeResult status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
