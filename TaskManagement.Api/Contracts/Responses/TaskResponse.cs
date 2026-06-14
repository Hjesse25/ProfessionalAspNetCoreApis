namespace TaskManagement.Api.Contracts.Responses;

public sealed record TaskResponse(
    int Id,
    string Title,
    string? Description,
    string Status,
    DateTime CreatedAtUtc
);