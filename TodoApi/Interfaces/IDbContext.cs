using Microsoft.EntityFrameworkCore;
using TodoApi.Models.Items;
using TodoApi.Models.Lists;

namespace TodoApi.Interfaces
{
    public interface IDbContext
    {
        #region Todo Lists
        DbSet<TodoList> TodoList { get; }
        #endregion

        #region Todo Items
        DbSet<TodoItem> TodoItem { get; }
        #endregion

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
