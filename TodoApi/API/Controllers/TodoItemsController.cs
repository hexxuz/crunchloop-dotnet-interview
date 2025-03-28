using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.API.Controllers
{
    [Route("todoitems/{listId}")]
    [ApiController]
    public class TodoItemsController : BaseController
    {
        private readonly IDbContext _context;

        public TodoItemsController(IDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IList<TodoList>>> GetTodoItems(long listId, CancellationToken cancellationToken = default)
        {
            TodoList? list = await _context.TodoList
                .Where(l => l.Id == listId)
                .FirstOrDefaultAsync(cancellationToken);

            return list is null ?
                NoContent() :
                Ok(list.TodoItems);
        }
    }
}