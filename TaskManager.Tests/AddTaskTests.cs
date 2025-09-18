using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Commands.AddTask;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.Tests;

public class AddTaskTests
{
    private readonly ITaskRepository _repo = new InMemoryTaskRepository();

    [Fact]
    public async Task AddTask_WithValidData_AddsTask()
    {
        // Arrange
        var handler = new AddTaskCommandHandler(_repo);
        var cmd = new AddTaskCommand("Učiti C#", DateTime.UtcNow.AddDays(1));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Učiti C#", result.Description);
        Assert.False(result.IsCompleted);
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}
