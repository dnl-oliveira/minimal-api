using MinimalApi.Data;
using MinimalApi.ViewModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Ok(new Todo(Guid.NewGuid(), "Ir a academia", false)));

app.MapGet("/v1/todos", (AppDbContext context) =>
{
    var todos = context.Todos;
    return todos is not null ? Results.Ok(todos) : Results.NotFound();
}).Produces<Todo>();

app.MapPost("/v1/todos", (AppDbContext context, CreateTodoViewModel model) =>
{
    var todo = model.MapTo();
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    context.Todos.Add(todo);
    context.SaveChanges();

    return Results.Created($"/v1/todos/{todo.Id}", todo);
});

app.Run();