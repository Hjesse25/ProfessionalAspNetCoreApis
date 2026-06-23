using TaskManagement.Api.Models;
using TaskManagement.Api.Contracts.Requests;
using TaskManagement.Api.Contracts.Responses;
using TaskManagement.Api.Mapping;
using TaskManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Validation;
using TaskManagement.Api.Constants;

var builder = WebApplication.CreateBuilder(args);

// Register services here
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

var app = builder.Build();

// Add middleware and map endpoints here
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

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
        return Results.Problem(
            title: "Task not found",
            detail: $"Task with ID {id} was not found.",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    return Results.Ok(task.ToReponse());
});

// create new task
app.MapPost("/api/tasks", async (
    CreateTaskRequest request,
    TaskManagementDbContext db) =>
{
    var validationError = TaskValidation.ValidateCreate(request);

    if (validationError is not null)
    {
        return Results.Problem(
            title: "Invalid task request",
            detail: validationError,
            statusCode: StatusCodes.Status400BadRequest
        );
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
        return Results.Problem(
            title: "Task not found",
            detail: $"Task with ID {id} was not found.",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    var validationError = TaskValidation.ValidateUpdate(request);

    if (validationError is not null)
    {
        return Results.Problem(
            title: "Invalid task request",
            detail: validationError,
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    task.Title = request.Title.Trim();
    task.Description = request.Description;
    task.Status = request.Status.Trim();

    await db.SaveChangesAsync();

    return Results.Ok(task.ToReponse());
});

app.MapDelete("/api/tasks/{id:int}", async (
    int id,
    TaskManagementDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.Problem(
            title: "Task not found.",
            detail: $"Task with ID {id} was not found.",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();