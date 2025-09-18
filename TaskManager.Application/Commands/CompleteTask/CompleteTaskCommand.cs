using MediatR;

namespace TaskManager.Application.Commands.CompleteTask;

public record CompleteTaskCommand(Guid TaskId) : IRequest<bool>;
