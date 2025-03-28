using TodoApi.Domain.Models.Lists;

namespace TodoApi.Domain.Models.Items
{
    public class TodoItem
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Foreign keys
        public virtual TodoList List { get; set; }
        public long ListId { get; set; }
    }
}