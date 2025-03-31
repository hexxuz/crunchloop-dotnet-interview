using System.Collections.Concurrent;
using TodoApi.Infrastructure.Interfaces;

namespace TodoApi.Infrastructure.Helpers
{
    public class BackgroundTodoItemsBulkDeleteQueue : IBackgroundTodoItemsBulkDeleteQueue
    {
        private readonly ConcurrentQueue<long> _queue = new();

        public void Enqueue(long listId)
        {
            _queue.Enqueue(listId);
        }

        public bool TryDequeue(out long listId)
        {
            return _queue.TryDequeue(out listId);
        }
    }
}
