using FluentValidation;
using TaskBoardApi.DTOs.Task;

namespace TaskBoardApi.Validators.Task
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(t => t.Title)
                .MaximumLength(50).WithMessage("Title must not exceed 50 characters.")
                .When(t => t.Title != null);

            RuleFor(t => t.Description)
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters.")
                .When(t => t.Description != null);

            RuleFor(t => t.Deadline)
                .GreaterThan(DateTime.Now.AddDays(1))
                    .WithMessage("Deadline must be at least 1 day in the future.")
                .When(t => t.Deadline != null);

            RuleFor(t => t.UserId)
                .GreaterThan(0).WithMessage("User id must be greater than 0.")
                .When(t => t.UserId.HasValue);
        }
    }
}
