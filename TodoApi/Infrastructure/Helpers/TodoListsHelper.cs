using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Infrastructure.Helpers
{
    public class TodoListsHelper : ITodoListsHelper
    {
        private readonly IDbContext _context;

        public TodoListsHelper(IDbContext context)
        {
            _context = context;
        }

        public async Task<TodoList?> GetTodoList(long id, bool includeItems, bool trackEntities, CancellationToken cancellationToken)
        {
            IQueryable<TodoList> query = _context.TodoList
                .Where(list => list.Id == id);

            if (includeItems)
                query = query.Include(list => list.TodoItems);

            query = trackEntities ?
                query.AsTracking() :
                query.AsNoTracking();

            TodoList? list = await query.FirstOrDefaultAsync();

            return list;
        }

        public Task<bool> TodoListExists(long id, CancellationToken cancellationToken)
        {
            return _context.TodoList.AnyAsync(list => list.Id == id, cancellationToken);
        }
    }
}
