using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public sealed class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(
        DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}