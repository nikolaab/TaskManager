using MediatR;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Commands.AddTask;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, TaskItem>
{
    private readonly ITaskRepository _repository;

    public AddTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskItem> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItem(request.Description, request.DueDate);
        return await _repository.AddAsync(task, cancellationToken);
    }
}
