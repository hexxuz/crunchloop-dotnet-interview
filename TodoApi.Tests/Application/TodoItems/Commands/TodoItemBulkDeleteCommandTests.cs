using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Tests.Application.TodoItems.Commands;

public class BulkDeleteTodoItemsCommandHandlerTests
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
        IBackgroundTodoItemsBulkDeleteQueue queue = new BackgroundTodoItemsBulkDeleteQueue();
        ITodoListsHelper helper = new TodoListsHelper(context);

        BulkDeleteTodoItemsCommandHandler handler = new BulkDeleteTodoItemsCommandHandler(queue, helper);

        BulkDeleteTodoItemsCommand command = new BulkDeleteTodoItemsCommand { ListId = 1 };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldEnqueueList_WhenListExists()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List" };

        context.TodoList.Add(list);
        await context.SaveChangesAsync();

        BackgroundTodoItemsBulkDeleteQueue queue = new BackgroundTodoItemsBulkDeleteQueue();
        TodoListsHelper helper = new TodoListsHelper(context);

        BulkDeleteTodoItemsCommandHandler handler = new BulkDeleteTodoItemsCommandHandler(queue, helper);

        BulkDeleteTodoItemsCommand command = new BulkDeleteTodoItemsCommand { ListId = list.Id };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<OkResult>(result);

        bool dequeued = queue.TryDequeue(out long dequeuedId);
        Assert.True(dequeued);
        Assert.Equal(list.Id, dequeuedId);
    }

    [Fact]
    public async Task Handle_ShouldReturn500_OnException()
    {
        TodoContext context = CreateContext();
        context.Dispose();

        IBackgroundTodoItemsBulkDeleteQueue queue = new BackgroundTodoItemsBulkDeleteQueue();
        ITodoListsHelper helper = new TodoListsHelper(context);

        BulkDeleteTodoItemsCommandHandler handler = new BulkDeleteTodoItemsCommandHandler(queue, helper);

        BulkDeleteTodoItemsCommand command = new BulkDeleteTodoItemsCommand { ListId = 1 };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        StatusCodeResult status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
