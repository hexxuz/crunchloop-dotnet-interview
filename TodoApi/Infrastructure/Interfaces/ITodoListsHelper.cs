using TodoApi.Domain.Models.Lists;

namespace TodoApi.Infrastructure.Interfaces
{
    public interface ITodoListsHelper
    {
        Task<TodoList?> GetTodoList(long id, bool includeItems = false, CancellationToken cancellationToken = default);

        Task<bool> TodoListExists(long id, CancellationToken cancellationToken = default);
    }
}
