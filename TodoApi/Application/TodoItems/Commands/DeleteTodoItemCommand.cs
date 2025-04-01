using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Commands
{
    public class DeleteTodoItemCommand : IRequest<ActionResult>
    {
        public long ListId { get; set; }
        public long ItemId { get; set; }
    }

    public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, ActionResult>
    {
        private readonly IDbContext _context;
        private readonly ITodoListsHelper _todoListsHelper;

        public DeleteTodoItemCommandHandler(IDbContext context, ITodoListsHelper todoListsHelper)
        {
            _context = context;
            _todoListsHelper = todoListsHelper;
        }

        public async Task<ActionResult> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _todoListsHelper.TodoListExists(request.ListId, cancellationToken))
                    return new NotFoundResult();

                TodoItem? todoItem = await _context.TodoItem
                    .Where(item => item.Id == request.ItemId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (todoItem is null)
                    return new NotFoundResult();

                if (todoItem.ListId != request.ListId)
                    return new NotFoundResult();

                _context.TodoItem.Remove(todoItem);

                await _context.SaveChangesAsync(cancellationToken);

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                // Analyze and define how to handle exceptions.
                return new StatusCodeResult(500);
            }
        }
    }
}
