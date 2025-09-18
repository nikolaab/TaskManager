using MediatR;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Queries.GetTasks;

public class GetOverdueTasksQueryHandler : IRequestHandler<GetOverdueTasksQuery, IReadOnlyList<TaskItem>>
{
    private readonly ITaskRepository _repository;

    public GetOverdueTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TaskItem>> Handle(GetOverdueTasksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetOverdueAsync(DateTime.UtcNow, cancellationToken);
    }
}
