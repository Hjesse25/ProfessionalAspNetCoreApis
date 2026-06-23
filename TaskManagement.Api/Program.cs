using TaskManagement.Api.Models;
using TaskManagement.Api.Contracts.Requests;
using TaskManagement.Api.Contracts.Responses;
using TaskManagement.Api.Mapping;
using TaskManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register services here
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")   
    );
});

var app = builder.Build();

// Add middleware and map endpoints here
app.UseHttpsRedirection();

// GET all tasks
app.MapGet("/api/tasks", async (TaskManagementDbContext db) =>
{
    var tasks = await db.Tasks
        .OrderByDescending(task => task.CreatedAtUtc)
        .ToListAsync();

    var response = tasks
        .Select(task => task.ToReponse())
        .ToList();

    return Results.Ok(response);
});

// Get task by id
app.MapGet("/api/tasks/{id:int}", async (
    int id,
    TaskManagementDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.NotFound(new
        {
            error = $"Task with ID {id} was not found."
        });
    }

    return Results.Ok(task.ToReponse());
});

// create new task
app.MapPost("/api/tasks", async (
    CreateTaskRequest request,
    TaskManagementDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new
        {
            error = "The task title is required."
        });
    }

    var task = new TaskItem
    {
        Title = request.Title.Trim(),
        Description = request.Description,
        Status = string.IsNullOrWhiteSpace(request.Status)
            ? "Pending"
            : request.Status.Trim(),
        CreatedAtUtc = DateTime.UtcNow
    };

    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{task.Id}", task.ToReponse());
});

// update task by id
app.MapPut("/api/tasks/{id:int}", async (
    int id, 
    UpdateTaskRequest request,
    TaskManagementDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

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

    await db.SaveChangesAsync();

    var response = new TaskResponse(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.CreatedAtUtc
    );

    return Results.Ok(response);
});

app.MapDelete("/api/tasks/{id:int}", async (
    int id,
    TaskManagementDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.NotFound(new
        {
            error = $"Task with ID {id} was not found."
        });
    }

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();