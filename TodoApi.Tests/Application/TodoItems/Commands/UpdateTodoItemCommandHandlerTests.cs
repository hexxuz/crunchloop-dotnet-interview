using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Application.TodoItems.Commands;

public class UpdateTodoItemCommandHandlerTests
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
        UpdateTodoItemCommandHandler handler = new UpdateTodoItemCommandHandler(context, new TodoListsHelper(context));

        UpdateTodoItemCommand command = new UpdateTodoItemCommand
        {
            ListId = 1,
            ItemId = 1,
            Name = "Updated",
            IsCompleted = true
        };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        TodoContext context = CreateContext();
        context.TodoList.Add(new TodoList { Id = 1, Name = "List" });
        await context.SaveChangesAsync();

        UpdateTodoItemCommandHandler handler = new UpdateTodoItemCommandHandler(context, new TodoListsHelper(context));

        UpdateTodoItemCommand command = new UpdateTodoItemCommand
        {
            ListId = 1,
            ItemId = 99,
            Name = "Updated",
            IsCompleted = true
        };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemNotInList()
    {
        TodoContext context = CreateContext();

        TodoList list1 = new TodoList { Id = 1, Name = "List1" };
        TodoList list2 = new TodoList { Id = 2, Name = "List2" };
        TodoItem item = new TodoItem { Id = 1, Name = "Item", List = list2 };

        context.TodoList.AddRange(list1, list2);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync();

        UpdateTodoItemCommandHandler handler = new UpdateTodoItemCommandHandler(context, new TodoListsHelper(context));

        UpdateTodoItemCommand command = new UpdateTodoItemCommand
        {
            ListId = 1,
            ItemId = 1,
            Name = "Updated",
            IsCompleted = true
        };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldUpdateItem_WhenValid()
    {
        TodoContext context = CreateContext();

        TodoList list = new TodoList { Name = "List" };
        TodoItem item = new TodoItem { Name = "Original", IsCompleted = false, List = list };

        context.TodoList.Add(list);
        context.TodoItem.Add(item);
        await context.SaveChangesAsync();

        UpdateTodoItemCommandHandler handler = new UpdateTodoItemCommandHandler(context, new TodoListsHelper(context));

        UpdateTodoItemCommand command = new UpdateTodoItemCommand
        {
            ListId = list.Id,
            ItemId = item.Id,
            Name = "Updated",
            IsCompleted = true
        };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        TodoItemDTO dto = Assert.IsType<TodoItemDTO>(ok.Value);

        Assert.Equal("Updated", dto.Name);
        Assert.True(dto.IsCompleted);

        TodoItem? updated = await context.TodoItem.FirstOrDefaultAsync();
        Assert.Equal("Updated", updated.Name);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task Handle_ShouldReturn500_OnException()
    {
        TodoContext context = CreateContext();
        TodoListsHelper helper = new TodoListsHelper(context);

        context.Dispose();

        UpdateTodoItemCommandHandler handler = new UpdateTodoItemCommandHandler(context, helper);

        UpdateTodoItemCommand command = new UpdateTodoItemCommand
        {
            ListId = 1,
            ItemId = 1,
            Name = "Error",
            IsCompleted = false
        };

        ActionResult result = await handler.Handle(command, CancellationToken.None);

        StatusCodeResult status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}
