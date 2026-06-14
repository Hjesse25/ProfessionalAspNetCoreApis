using TaskManagement.Api.Contracts.Responses;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Mapping;

public static class TaskMappings
{
    public static TaskResponse ToReponse(this TaskItem task)
    {
        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.CreatedAtUtc
        );
    }
}