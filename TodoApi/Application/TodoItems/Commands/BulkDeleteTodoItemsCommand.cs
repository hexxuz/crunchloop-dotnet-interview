using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Commands
{
    public class BulkDeleteTodoItemsCommand : IRequest<ActionResult>
    {
        public long ListId { get; set; }
    }

    public class BulkDeleteTodoItemsCommandHandler : IRequestHandler<BulkDeleteTodoItemsCommand, ActionResult>
    {
        private readonly IBackgroundTodoItemsBulkDeleteQueue _bulkDeleteQueue;
        private readonly ITodoListsHelper _todoListsHelper;

        public BulkDeleteTodoItemsCommandHandler(IBackgroundTodoItemsBulkDeleteQueue bulkDeleteQueue, ITodoListsHelper todoListsHelper)
        {
            _bulkDeleteQueue = bulkDeleteQueue;
            _todoListsHelper = todoListsHelper;
        }

        public async Task<ActionResult> Handle(BulkDeleteTodoItemsCommand request, CancellationToken cancellationToken)
        {
            if (!await _todoListsHelper.TodoListExists(request.ListId, cancellationToken))
                return new NotFoundResult();

            _bulkDeleteQueue.Enqueue(request.ListId);

            return new OkResult();
        }
    }
}
