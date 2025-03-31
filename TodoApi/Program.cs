using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TodoApi.Infrastructure.Helpers;
using TodoApi.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddDbContext<TodoContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"))
    )
    .AddScoped<IDbContext, TodoContext>()
    .AddScoped<ITodoListsHelper, TodoListsHelper>()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddEndpointsApiExplorer()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
    .Services.AddCors(options =>
    {
        options.AddPolicy(name: "FrontEndUI", policy =>
        {
            policy.WithOrigins("http://localhost:4200/").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
        });
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("FrontEndUI");
}

app.UseAuthorization();
app.MapControllers();
app.Run();
