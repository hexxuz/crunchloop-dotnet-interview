namespace TodoApi.Infrastructure.Interfaces
{
    public interface IBackgroundTodoItemsBulkDeleteQueue
    {
        void Enqueue(long listId);
        bool TryDequeue(out long listId);
    }
}