using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Abstractions;

public interface ITaskRepository
{
    // Komande (write)
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> MarkCompleteAsync(Guid id, CancellationToken ct = default);

    // Upiti (read)
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken ct = default);

    // Dodatno (korisno za Workera)
    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(DateTime nowUtc, CancellationToken ct = default);
}
