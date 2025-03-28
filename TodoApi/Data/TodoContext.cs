using Microsoft.EntityFrameworkCore;
using TodoApi.Interfaces;
using TodoApi.Models.Items;
using TodoApi.Models.Lists;

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
