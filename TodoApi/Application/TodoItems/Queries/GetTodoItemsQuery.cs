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
        private readonly ITodoListsHelper _todoListsHelper;

        public GetTodoItemsQueryHandler(ITodoListsHelper todoListsHelper)
        {
            _todoListsHelper = todoListsHelper;
        }

        public async Task<ActionResult> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                TodoList? list = await _todoListsHelper.GetTodoList(request.ListId, true, false, cancellationToken);

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
            catch (Exception ex)
            {
                // Analyze and define how to handle exceptions.
                return new StatusCodeResult(500);
            }
        }
    }
}