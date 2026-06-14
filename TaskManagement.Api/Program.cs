using TaskManagement.Api.Models;
using TaskManagement.Api.Contracts.Requests;
using TaskManagement.Api.Contracts.Responses;
using TaskManagement.Api.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Register services here

var app = builder.Build();

// temporary storage of tasks
var tasks = new List<TaskItem>();

// Add middleware and map endpoints here
app.UseHttpsRedirection();

app.MapGet("/api/tasks", () =>
{
    var response = tasks
        .Select(task => task.ToReponse())
        .ToList();

    return Results.Ok(response);
});

app.MapGet("/api/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(task => task.Id == id);

    if (task is null)
    {
        return Results.NotFound(new
        {
            error = $"Task with ID {id} was not found."
        });
    }

    return Results.Ok(task.ToReponse());
});

app.MapPost("/api/tasks", (CreateTaskRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new
        {
            error = "The task title is required."
        });
    }

    var nextId = tasks.Count == 0
        ? 1
        : tasks.Max(t => t.Id) + 1;

    var task = new TaskItem
    {
        Id = nextId,
        Title = request.Title.Trim(),
        Description = request.Description,
        Status = string.IsNullOrWhiteSpace(request.Status)
            ? "Pending"
            : request.Status.Trim(),
        CreatedAtUtc = DateTime.UtcNow
    };

    tasks.Add(task);

    return Results.Created($"/api/tasks/{task.Id}", task.ToReponse());
});

app.MapPut("/api/tasks/{id:int}", (int id, UpdateTaskRequest request) =>
{
    var task = tasks.FirstOrDefault(task => task.Id == id);

    if (task is null)
    {
        return Results.NotFound(new
        {
            error = $"Task with ID {id} was not found."
        });
    }

    task.Title = request.Title.Trim();
    task.Description = request.Description;
    task.Status = request.Status.Trim();

    var response = new TaskResponse(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.CreatedAtUtc
    );

    return Results.Ok(response);
});

app.Run();