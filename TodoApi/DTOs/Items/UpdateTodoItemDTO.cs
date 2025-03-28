namespace TodoApi.Dtos.Items
{
    public class UpdateTodoItemDTO
    {
        public required string Name { get; set; }
        public required bool IsCompleted { get; set; }
    }
}
