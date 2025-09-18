using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = new();

    public Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default)
    {
        _tasks.Add(task);
        return Task.FromResult(task);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return Task.FromResult(false);
        _tasks.Remove(task);
        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<TaskItem>)_tasks);

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

    public Task<bool> MarkCompleteAsync(Guid id, CancellationToken ct = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return Task.FromResult(false);
        task.Complete();
        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<TaskItem>> GetOverdueAsync(DateTime nowUtc, CancellationToken ct = default)
    {
        var overdue = _tasks
            .Where(t => t.DueDate != null && t.DueDate < nowUtc && !t.IsCompleted)
            .ToList();
        return Task.FromResult((IReadOnlyList<TaskItem>)overdue);
    }
}
