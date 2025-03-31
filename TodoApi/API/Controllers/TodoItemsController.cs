using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Application.TodoItems.Queries;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.API.Controllers
{
    [Route("api/todolists/{listId}/todoitems")]
    [ApiController]
    public class TodoItemsController : BaseController
    {
        private readonly IDbContext _context;

        public TodoItemsController(IDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> PostTodoItem(long listId, [FromBody] PostTodoItemCommand command, CancellationToken cancellationToken = default)
        {
            command.ListId = listId;

            return await Mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        public async Task<ActionResult> GetTodoItems(long listId, CancellationToken cancellationToken = default)
        {
            return await Mediator.Send(new GetTodoItemsQuery()
            {
                ListId = listId
            }, cancellationToken);
        }

        [HttpGet("{itemId}")]
        public async Task<ActionResult> GetTodoItem(long listId, long itemId, CancellationToken cancellationToken = default)
        {
            return await Mediator.Send(new GetTodoItemQuery()
            {
                ListId = listId,
                ItemId = itemId
            }, cancellationToken);
        }

        [HttpPut("{itemId}")]
        public async Task<ActionResult> PutTodoItem(long listId, long itemId, [FromBody] UpdateTodoItemCommand command, CancellationToken cancellationToken = default)
        {
            command.ListId = listId;
            command.ItemId = itemId;

            return await Mediator.Send(command, cancellationToken);
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteTodoItem(long listId, long itemId, CancellationToken cancellationToken = default)
        {
            return await Mediator.Send(new DeleteTodoItemCommand()
            {
                ItemId = itemId,
                ListId = listId
            }, cancellationToken);
        }

        [HttpDelete("bulkdelete")]
        public async Task<ActionResult> DeleteTodoItemsBulk(long listId, CancellationToken cancellationToken = default)
        {
            return await Mediator.Send(new BulkDeleteTodoItemsCommand()
            {
                ListId = listId
            }, cancellationToken);
        }

        [HttpPost("bulkcreation")]
        public async Task<ActionResult> PostTodoItemsBulk(long listId, CancellationToken cancellationToken = default)
        {
            TodoList? list = await _context.TodoList.FirstOrDefaultAsync(list => list.Id == listId, cancellationToken);

            if (list is null)
                return new NotFoundResult();

            list.TodoItems = new List<TodoItem>();

            for (int i = 0; i < 5000; i++)
            {
                list.TodoItems.Add(new() { Name = i.ToString() });
            }

            _context.TodoList.Update(list);

            await _context.SaveChangesAsync();

            return new OkResult();
        }
    }
}