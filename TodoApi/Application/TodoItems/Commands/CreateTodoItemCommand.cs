using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Application.TodoItems.Commands
{
    public class CreateTodoItemCommand : IRequest<IActionResult>
    {
        public long ListId { get; set; }
        public required string Name { get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, IActionResult>
    {
        private readonly IDbContext _context;

        public CreateTodoItemCommandHandler(IDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                TodoList? list = await _context.TodoList
                    .Where(list => list.Id == request.ListId)
                    .Include(list => list.TodoItems)
                    .FirstOrDefaultAsync(cancellationToken);

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
                   value: newItem
               );
            }
            catch (Exception ex)
            {
                // Define how to handle exceptions.
                return new StatusCodeResult(500);
            }
        }
    }
}
