using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Application.TodoItems.Commands;

public class PostTodoItemCommandHandlerTests
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
        TodoListsHelper helper = new TodoListsHelper(context);
        PostTodoItemCommandHandler handler = new PostTodoItemCommandHandler(context, helper);

        PostTodoItemCommand command = new PostTodoItemCommand { ListId = 1, Name = "Test" };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Handle_ShouldAddItem_WhenListExists()
    {
        TodoContext context = CreateContext();
        TodoList list = new TodoList { Name = "List", TodoItems = new List<TodoItem>() };

        context.TodoList.Add(list);
        await context.SaveChangesAsync(CancellationToken.None);

        TodoListsHelper helper = new TodoListsHelper(context);
        PostTodoItemCommandHandler handler = new PostTodoItemCommandHandler(context, helper);

        PostTodoItemCommand command = new PostTodoItemCommand { ListId = list.Id, Name = "Test" };
        ActionResult result = await handler.Handle(command, CancellationToken.None);

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(result);
        TodoItemDTO dto = Assert.IsType<TodoItemDTO>(created.Value);

        Assert.Equal("Test", dto.Name);
        Assert.Equal(list.Id, dto.ListId);

        TodoItem? savedItem = await context.TodoItem.FirstOrDefaultAsync();
        Assert.NotNull(savedItem);
        Assert.Equal("Test", savedItem.Name);
    }

}
