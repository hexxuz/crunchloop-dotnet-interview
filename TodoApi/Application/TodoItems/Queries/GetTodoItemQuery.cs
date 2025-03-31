using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Items;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Queries
{
    public class GetTodoItemQuery : IRequest<ActionResult>
    {
        public long ListId { get; set; }
        public long ItemId { get; set; }
    }

    public class GetTodoItemQueryHandler : IRequestHandler<GetTodoItemQuery, ActionResult>
    {
        private readonly IDbContext _context;
        private readonly ITodoListsHelper _listsHelper;

        public GetTodoItemQueryHandler(IDbContext context, ITodoListsHelper listsHelper)
        {
            _context = context;
            _listsHelper = listsHelper;
        }

        public async Task<ActionResult> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            if (!await _listsHelper.TodoListExists(request.ListId, cancellationToken))
                return new NotFoundResult();

            TodoItem? item = await _context.TodoItem
                .Where(item => item.Id == request.ListId)
                .FirstOrDefaultAsync(cancellationToken);

            if (item is null)
                return new NotFoundResult();

            return new OkObjectResult(new TodoItemDTO()
            {
                Id = item.Id,
                IsCompleted = item.IsCompleted,
                Name = item.Name,
                ListId = item.ListId
            });
        }
    }
}
