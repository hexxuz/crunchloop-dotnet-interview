using TodoApi.Infrastructure.Helpers;

namespace TodoApi.Tests.Infrastructure.Helpers
{
    public class BackgroundTodoItemsBulkDeleteQueueTests
    {
        [Fact]
        public void Enqueue_Should_Add_Item_To_Queue()
        {
            BackgroundTodoItemsBulkDeleteQueue queue = new BackgroundTodoItemsBulkDeleteQueue();
            long listId = 123;

            queue.Enqueue(listId);

            bool success = queue.TryDequeue(out var dequeuedId);
            Assert.True(success);
            Assert.Equal(listId, dequeuedId);
        }

        [Fact]
        public void TryDequeue_Should_Return_False_When_Queue_Is_Empty()
        {
            BackgroundTodoItemsBulkDeleteQueue queue = new BackgroundTodoItemsBulkDeleteQueue();
            bool success = queue.TryDequeue(out var listId);

            Assert.False(success);
            Assert.Equal(0, listId);
        }
    }
}
