namespace TaskManagement.Api.Constants;

public static class TaskStatuses
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";

    public static readonly string[] All = [
        Pending,
        InProgress,
        Completed
    ];
}