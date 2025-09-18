using MediatR;
using TaskManager.Domain.Abstractions;

namespace TaskManager.Application.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITaskRepository _repository;

    public CompleteTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        return await _repository.MarkCompleteAsync(request.TaskId, cancellationToken);
    }
}
