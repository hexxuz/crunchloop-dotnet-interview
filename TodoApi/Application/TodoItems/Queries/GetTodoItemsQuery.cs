using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Queries
{
    public class GetTodoItemsQuery : IRequest<ActionResult>
    {
        public long ListId { get; set; }
    }

    public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, ActionResult>
    {
        private readonly IDbContext _context;

        public GetTodoItemsQueryHandler(IDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            TodoList? list = await _context.TodoList
                    .Where(list => list.Id == request.ListId)
                    .Include(list => list.TodoItems)
                    .FirstOrDefaultAsync(cancellationToken);

            if (list is null)
                return new NotFoundResult();

            if (list.TodoItems is null || !list.TodoItems.Any())
                return new OkResult();

            List<TodoItemDTO> items = list.TodoItems.Select(item =>
            {
                return new TodoItemDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsCompleted = item.IsCompleted
                };
            }).ToList();

            return new OkObjectResult(items);
        }
    }
}