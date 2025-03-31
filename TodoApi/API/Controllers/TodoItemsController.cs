using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.TodoItems.Commands;
using TodoApi.Application.TodoItems.Queries;

namespace TodoApi.API.Controllers
{
    [Route("api/todolists/{listId}/todoitems")]
    [ApiController]
    public class TodoItemsController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult> PostTodoItem(long listId, [FromBody] CreateTodoItemCommand command, CancellationToken cancellationToken = default)
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

        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteTodoItem(long listId, long itemId, CancellationToken cancellationToken = default)
        {
            return await Mediator.Send(new DeleteTodoItemCommand()
            {
                ItemId = listId,
                ListId = itemId
            }, cancellationToken);
        }
    }
}