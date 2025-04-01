using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Items;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Commands
{
    public class UpdateTodoItemCommand : IRequest<ActionResult>
    {
        public long ListId { get; set; }
        public long ItemId { get; set; }
        public required string Name { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, ActionResult>
    {
        private readonly IDbContext _context;
        private readonly ITodoListsHelper _listsHelper;

        public UpdateTodoItemCommandHandler(IDbContext context, ITodoListsHelper listsHelper)
        {
            _context = context;
            _listsHelper = listsHelper;
        }

        public async Task<ActionResult> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _listsHelper.TodoListExists(request.ListId, cancellationToken))
                    return new NotFoundResult();

                TodoItem? item = await _context.TodoItem
                    .Where(item => item.Id == request.ItemId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (item is null)
                    return new NotFoundResult();

                if (item.ListId != request.ListId)
                    return new NotFoundResult();

                item.IsCompleted = request.IsCompleted;
                item.Name = request.Name;

                _context.TodoItem.Update(item);

                await _context.SaveChangesAsync(cancellationToken);

                return new OkObjectResult(new TodoItemDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsCompleted = item.IsCompleted,
                    ListId = item.ListId
                });
            }
            catch (Exception ex)
            {
                // Analyze and define how to handle exceptions.
                return new StatusCodeResult(500);
            }
        }
    }
}
