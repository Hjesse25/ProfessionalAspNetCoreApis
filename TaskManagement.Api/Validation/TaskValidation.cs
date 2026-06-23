using TaskManagement.Api.Constants;
using TaskManagement.Api.Contracts.Requests;

namespace TaskManagement.Api.Validation;

public static class TaskValidation
{
    public static string? ValidateCreate(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return "The task title is required";
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && 
            !TaskStatuses.All.Contains(request.Status.Trim()))
        {
            return "The task status is invalid";
        }

        return null;
    }

    public static string? ValidateUpdate(UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return "The task title is required";
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return "The task status is required";
        }

        if (!TaskStatuses.All.Contains(request.Status.Trim()))
        {
            return "The task status is invalid";
        }

        return null;
    }
}