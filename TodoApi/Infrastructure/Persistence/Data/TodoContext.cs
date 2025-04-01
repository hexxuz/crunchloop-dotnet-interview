using Microsoft.EntityFrameworkCore;
using TodoApi.Domain.Models.Items;
using TodoApi.Domain.Models.Lists;
using TodoApi.Infrastructure.Interfaces;

public class TodoContext : DbContext, IDbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }


    #region Todo Lists
    public DbSet<TodoList> TodoList => Set<TodoList>();
    #endregion

    #region Todo Items
    public DbSet<TodoItem> TodoItem => Set<TodoItem>();
    #endregion
}
