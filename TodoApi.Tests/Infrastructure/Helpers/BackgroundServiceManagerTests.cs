using Microsoft.Extensions.DependencyInjection;
using Moq;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Helpers;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Tests.Infrastructure.Helpers;

public class BackgroundServiceManagerTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeleteTodoItems_WhenListHasItems()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(500);

        long listId = 1;

        Mock<IBackgroundTodoItemsBulkDeleteQueue> queueMock = new Mock<IBackgroundTodoItemsBulkDeleteQueue>();
        queueMock.SetupSequence(q => q.TryDequeue(out listId))
            .Returns(true)
            .Returns(false);

        List<TodoItem> items = new List<TodoItem>
        {
            new TodoItem { Id = 1, Name = "Item", IsCompleted = false, ListId = listId }
        };

        TodoList todoList = new TodoList { Id = listId, Name = "List", TodoItems = items };

        Mock<ITodoListsHelper> helperMock = new Mock<ITodoListsHelper>();
        helperMock.Setup(h => h.GetTodoList(listId, true, true, It.IsAny<CancellationToken>())).ReturnsAsync(todoList);

        Mock<IDbContext> dbContextMock = new Mock<IDbContext>();
        dbContextMock.Setup(d => d.TodoItem).Returns(Mock.Of<Microsoft.EntityFrameworkCore.DbSet<TodoItem>>());

        Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(s => s.GetService(typeof(IDbContext))).Returns(dbContextMock.Object);
        serviceProviderMock.Setup(s => s.GetService(typeof(ITodoListsHelper))).Returns(helperMock.Object);

        Mock<IServiceScope> scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        Mock<IServiceScopeFactory> scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        serviceProviderMock.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactoryMock.Object);

        BackgroundServiceManager service = new BackgroundServiceManager(serviceProviderMock.Object, queueMock.Object);

        await service.StartAsync(cancellationTokenSource.Token);

        dbContextMock.Verify(d => d.TodoItem.RemoveRange(items), Times.Once);
        dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
