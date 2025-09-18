using MediatR;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Commands.AddTask;

public record AddTaskCommand(string Description, DateTime? DueDate) 
    : IRequest<TaskItem>;
