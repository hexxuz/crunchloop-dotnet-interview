using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ITodoListsHelper _todoListsHelper;

        public GetTodoItemsQueryHandler(IDbContext context, ITodoListsHelper todoListsHelper)
        {
            _context = context;
            _todoListsHelper = todoListsHelper;
        }

        public async Task<ActionResult> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            TodoList? list = await _todoListsHelper.GetTodoList(request.ListId, true, cancellationToken);

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
                    IsCompleted = item.IsCompleted,
                    ListId = list.Id
                };
            }).ToList();

            return new OkObjectResult(items);
        }
    }
}