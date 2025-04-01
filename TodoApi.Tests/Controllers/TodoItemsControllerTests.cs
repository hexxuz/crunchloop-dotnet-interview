using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TodoApi.API.Controllers;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Application.TodoItems.Queries;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Tests.Controllers;

public class TodoItemsControllerTests
{
    [Fact]
    public async Task PostTodoItem_ShouldSendCommandWithCorrectListId()
    {
        long listId = 1;
        PostTodoItemCommand command = new PostTodoItemCommand { Name = "Test" };

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<PostTodoItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResult());

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.PostTodoItem(listId, command);

        Assert.Equal(listId, command.ListId);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetTodoItems_ShouldSendQueryWithCorrectListId()
    {
        long listId = 2;

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<GetTodoItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkObjectResult("items"));

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.GetTodoItems(listId);

        senderMock.Verify(s => s.Send(
            It.Is<GetTodoItemsQuery>(q => q.ListId == listId),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetTodoItem_ShouldSendQueryWithCorrectItemAndListIds()
    {
        long listId = 3;
        long itemId = 5;

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<GetTodoItemQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkObjectResult("item"));

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.GetTodoItem(listId, itemId);

        senderMock.Verify(s => s.Send(
            It.Is<GetTodoItemQuery>(q => q.ListId == listId && q.ItemId == itemId),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task PutTodoItem_ShouldSendCommandWithCorrectIds()
    {
        long listId = 4;
        long itemId = 8;
        UpdateTodoItemCommand command = new UpdateTodoItemCommand { Name = "Test", IsCompleted = false };

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<UpdateTodoItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResult());

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.PutTodoItem(listId, itemId, command);

        Assert.Equal(listId, command.ListId);
        Assert.Equal(itemId, command.ItemId);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ShouldSendCommandWithCorrectItemAndListIds()
    {
        long listId = 3;
        long itemId = 9;

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<DeleteTodoItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResult());

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.DeleteTodoItem(listId, itemId);

        senderMock.Verify(s => s.Send(
            It.Is<DeleteTodoItemCommand>(c => c.ListId == listId && c.ItemId == itemId),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItemsBulk_ShouldSendCommandWithCorrectListId()
    {
        long listId = 10;

        Mock<ISender> senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<BulkDeleteTodoItemsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OkResult());

        Mock<IDbContext> contextMock = new Mock<IDbContext>();

        TodoItemsController controller = new TodoItemsController(contextMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(senderMock.Object)
            .BuildServiceProvider();

        IActionResult result = await controller.DeleteTodoItemsBulk(listId);

        senderMock.Verify(s => s.Send(
            It.Is<BulkDeleteTodoItemsCommand>(c => c.ListId == listId),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsType<OkResult>(result);
    }
}
