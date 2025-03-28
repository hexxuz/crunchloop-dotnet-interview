namespace TodoApi.Models.Items
{
    public class TodoItem
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required int ListId { get; set; }
    }
}