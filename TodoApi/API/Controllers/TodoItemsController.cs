using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.TodoItems.Commands;

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
    }
}