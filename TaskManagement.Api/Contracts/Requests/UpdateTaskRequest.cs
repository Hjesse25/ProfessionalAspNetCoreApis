namespace TaskManagement.Api.Contracts.Requests;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    string Status
);