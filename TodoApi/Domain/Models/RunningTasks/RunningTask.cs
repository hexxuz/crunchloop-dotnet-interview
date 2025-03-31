using TodoApi.Domain.Models.Lists;

namespace TodoApi.Domain.Models.RunningTasks
{
    public class RunningTask
    {
        public long Id { get; set; }
        public DateTime StartingDate { get; set; }

        public virtual TodoList List { get; set; }
        public long ListId { get; set; }
    }
}