using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;

namespace TodoApi.Infrastructure.Interfaces
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
