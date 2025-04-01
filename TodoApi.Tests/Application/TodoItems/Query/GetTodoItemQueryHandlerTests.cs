using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Application.TodoItems.Queries;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Application.TodoItems.Queries;

public class GetTodoItemQueryHandlerTests
{
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
        GetTodoItemQueryHandler handler = new GetTodoItemQueryHandler(context, new TodoListsHelper(context));

        GetTodoItemQuery query = new GetTodoItemQuery { ListId = 1, ItemId = 1 };
        ActionResult result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        TodoContext context = CreateContext();
        context.TodoList.Add(new TodoList { Id = 1, Name = "List" });
        await context.SaveChangesAsync();

        GetTodoItemQueryHandler handler = new GetTodoItemQueryHandler(context, new TodoListsHelper(context));

        GetTodoItemQuery query = new GetTodoItemQuery { ListId = 1, ItemId = 999 };
        ActionResult result = await handler.Handle(query, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnItem_WhenFound()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Test", IsCompleted = false, List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync();

        GetTodoItemQueryHandler handler = new GetTodoItemQueryHandler(context, new TodoListsHelper(context));

        GetTodoItemQuery query = new GetTodoItemQuery { ListId = list.Id, ItemId = item.Id };
        ActionResult result = await handler.Handle(query, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        TodoItemDTO dto = Assert.IsType<TodoItemDTO>(ok.Value);

        Assert.Equal(item.Id, dto.Id);
        Assert.Equal(item.Name, dto.Name);
        Assert.Equal(item.IsCompleted, dto.IsCompleted);
        Assert.Equal(item.ListId, dto.ListId);
    }

    [Fact]
    public async Task Handle_ShouldReturn500_OnException()
    {
        TodoContext context = CreateContext();
        context.Dispose();

        GetTodoItemQueryHandler handler = new GetTodoItemQueryHandler(context, new TodoListsHelper(context));

        GetTodoItemQuery query = new GetTodoItemQuery { ListId = 1, ItemId = 1 };
        ActionResult result = await handler.Handle(query, CancellationToken.None);

        StatusCodeResult status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
