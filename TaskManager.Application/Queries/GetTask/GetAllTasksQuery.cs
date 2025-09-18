using MediatR;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Queries.GetTasks;

public record GetAllTasksQuery() : IRequest<IReadOnlyList<TaskItem>>;
