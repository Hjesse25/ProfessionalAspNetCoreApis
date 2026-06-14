namespace TaskManagement.Api.Contracts.Requests;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string? Status
);