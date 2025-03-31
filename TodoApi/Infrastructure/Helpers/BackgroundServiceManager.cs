using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Infrastructure.Helpers
{
    public class BackgroundServiceManager : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTodoItemsBulkDeleteQueue _queue;

        public BackgroundServiceManager(IServiceProvider serviceProvider, IBackgroundTodoItemsBulkDeleteQueue queue)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out long listId))
                {
                    try
                    {
                        using IServiceScope scope = _serviceProvider.CreateScope();

                        IDbContext dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                        ITodoListsHelper todoListsHelper = scope.ServiceProvider.GetRequiredService<ITodoListsHelper>();

                        TodoList? list = await todoListsHelper.GetTodoList(listId, true, true, cancellationToken);

                        if (list?.TodoItems?.Count > 0)
                        {
                            dbContext.TodoItem.RemoveRange(list.TodoItems);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        // ToDo: Manage exceptions
                    }
                }
                else
                {
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }
    }
}