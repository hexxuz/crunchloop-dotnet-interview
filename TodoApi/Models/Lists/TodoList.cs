using TodoApi.Models.Items;

namespace TodoApi.Models.Lists;

public class TodoList
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<TodoItem>? TodoItems { get; set; }
}
