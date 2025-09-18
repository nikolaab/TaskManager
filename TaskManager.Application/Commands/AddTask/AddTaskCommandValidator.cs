using FluentValidation;

namespace TaskManager.Application.Commands.AddTask;

public class AddTaskCommandValidator : AbstractValidator<AddTaskCommand>
{
    public AddTaskCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(3).WithMessage("Description must be at least 3 characters long.");

        RuleFor(x => x.DueDate)
            .Must(date => date == null || date > DateTime.UtcNow)
            .WithMessage("Due date must be in the future.");
    }
}
