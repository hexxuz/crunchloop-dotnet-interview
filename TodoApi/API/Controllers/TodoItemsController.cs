using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Interfaces;
using TodoApi.Models.Lists;

namespace TodoApi.Controllers
{
    [Route("todoitems/{listId}")]
    [ApiController]
    public class TodoItemsController : ControllerBase
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