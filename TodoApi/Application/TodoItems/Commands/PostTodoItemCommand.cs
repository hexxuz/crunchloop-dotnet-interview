using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Application.TodoItems.DTOs;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Commands
{
    public class PostTodoItemCommand : IRequest<ActionResult>
    {
        public long ListId { get; set; }
        public required string Name { get; set; }
    }

    public class PostTodoItemCommandHandler : IRequestHandler<PostTodoItemCommand, ActionResult>
    {
        private readonly IDbContext _context;
        private readonly ITodoListsHelper _todoListsHelper;

        public PostTodoItemCommandHandler(IDbContext context, ITodoListsHelper todoListsHelper)
        {
            _context = context;
            _todoListsHelper = todoListsHelper;
        }

        public async Task<ActionResult> Handle(PostTodoItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                TodoList? list = await _todoListsHelper.GetTodoList(request.ListId, true);

                if (list is null)
                    return new NotFoundResult();

                TodoItem newItem = new TodoItem()
                {
                    Name = request.Name
                };

                if (list.TodoItems is null)
                {
                    list.TodoItems = new List<TodoItem>() { newItem };
                }
                else
                {
                    list.TodoItems.Add(newItem);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new CreatedAtActionResult(
                   actionName: "PostTodoItem",
                   controllerName: "TodoItems",
                   routeValues: new { id = newItem.Id },

                   // ToDo: Implement auto mapper
                   value: new TodoItemDTO()
                   {
                       Id = newItem.Id,
                       Name = newItem.Name,
                       IsCompleted = newItem.IsCompleted,
                       ListId = list.Id
                   }
               );
            }
            catch (Exception ex)
            {
                // Analyze and define how to handle exceptions.
                return new StatusCodeResult(500);
            }
        }
    }
}
