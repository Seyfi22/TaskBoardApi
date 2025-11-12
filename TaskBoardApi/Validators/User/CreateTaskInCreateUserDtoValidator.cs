using FluentValidation;
using TaskBoardApi.DTOs.User;

namespace TaskBoardApi.Validators.User
{
    public class CreateTaskInCreateUserDtoValidator : AbstractValidator<CreateTaskInCreateUserDto>
    {
        public CreateTaskInCreateUserDtoValidator()
        {
            RuleFor(t => t.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(50).WithMessage("Title must not exceed 50 characters.");

            RuleFor(t => t.Description)
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters.")
                .When(t => !string.IsNullOrWhiteSpace(t.Description));

            RuleFor(t => t.Deadline)
                .NotEmpty().WithMessage("Deadline is required.")
                .GreaterThan(DateTime.Now.AddDays(1))
                    .WithMessage("Deadline must be at least 1 day in the future.");
        }
    }
}
