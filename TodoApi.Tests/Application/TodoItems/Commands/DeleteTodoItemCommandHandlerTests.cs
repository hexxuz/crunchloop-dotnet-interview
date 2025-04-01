using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Application.TodoItems.Commands;

public class DeleteTodoItemCommandHandlerTests
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
        DeleteTodoItemCommandHandler handler = new DeleteTodoItemCommandHandler(context, new TodoListsHelper(context));

        DeleteTodoItemCommand command = new DeleteTodoItemCommand { ListId = 1, ItemId = 1 };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        TodoContext context = CreateContext();
        context.TodoList.Add(new TodoList { Id = 1, Name = "List" });
        await context.SaveChangesAsync();

        DeleteTodoItemCommandHandler handler = new DeleteTodoItemCommandHandler(context, new TodoListsHelper(context));
        DeleteTodoItemCommand command = new DeleteTodoItemCommand { ListId = 1, ItemId = 99 };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemNotInList()
    {
        TodoContext context = CreateContext();

        TodoList list1 = new TodoList { Id = 1, Name = "List1" };
        TodoList list2 = new TodoList { Id = 2, Name = "List2" };
        TodoItem item = new TodoItem { Id = 100, Name = "Test", List = list2 };

        context.TodoList.AddRange(list1, list2);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync();

        DeleteTodoItemCommandHandler handler = new DeleteTodoItemCommandHandler(context, new TodoListsHelper(context));
        DeleteTodoItemCommand command = new DeleteTodoItemCommand { ListId = 1, ItemId = 100 };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoContent_WhenItemDeleted()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Test", List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync();

        DeleteTodoItemCommandHandler handler = new DeleteTodoItemCommandHandler(context, new TodoListsHelper(context));
        DeleteTodoItemCommand command = new DeleteTodoItemCommand { ListId = list.Id, ItemId = item.Id };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        bool stillExists = await context.TodoItem.AnyAsync(i => i.Id == item.Id);
        Assert.False(stillExists);
    }

    [Fact]
    public async Task Handle_ShouldReturn500_OnException()
    {
        TodoContext context = CreateContext();
        TodoListsHelper helper = new TodoListsHelper(context);

        context.Dispose();

        DeleteTodoItemCommandHandler handler = new DeleteTodoItemCommandHandler(context, helper);
        DeleteTodoItemCommand command = new DeleteTodoItemCommand { ListId = 1, ItemId = 1 };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        StatusCodeResult status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
