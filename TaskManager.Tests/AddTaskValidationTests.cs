using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TaskManager.Application.Commands.AddTask;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.Tests
{
    public class AddTaskValidationTests
    {
        private readonly ITaskRepository _repo = new InMemoryTaskRepository();

        [Fact]
        public async Task AddTask_WithEmptyDescription_ThrowsValidationException()
        {
            // Arrange
            var validator = new AddTaskCommandValidator();
            var handler = new AddTaskCommandHandler(_repo);
            var cmd = new AddTaskCommand("", DateTime.UtcNow.AddDays(1));

            // Act + Assert: simuliramo što radi ValidationBehavior
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = validator.Validate(cmd);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);

                // (neće doći dovde jer ćemo baciti izuzetak gore)
                await handler.Handle(cmd, CancellationToken.None);
            });

            Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("Description is required"));
        }
    }
}
