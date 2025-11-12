using FluentValidation;
using TaskBoardApi.Data;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Model.Enums;

namespace TaskBoardApi.Validators.User
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator(TaskBoardDbContext context)
        {
            RuleFor(u => u.Fullname)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .Must(e => !context.Users.Any(u => u.Email == e))
                    .WithMessage("This email has already been registered"); ;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches(@"^[A-Z]").WithMessage("Password must start with an uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(r => Enum.TryParse<Role>(r, true, out _))
                    .WithMessage($"Role must be one of the following: {string.Join(", ", Enum.GetNames(typeof(Role)))}");

            RuleForEach(u => u.Tasks)
                .SetValidator(new CreateTaskInCreateUserDtoValidator());
        }
    }
}
