using FluentValidation;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Model.Enums;

namespace TaskBoardApi.Validators.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(u => u.Fullname)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
                .When(u => u.Fullname != null);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .When(u => u.Email != null); // In Services folder the rest will be solved inshAllah :)

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches(@"^[A-Z]").WithMessage("Password must start with an uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .When(u => u.Password != null);

            RuleFor(x => x.Role)
                .Must(r => Enum.TryParse<Role>(r, true, out _))
                    .WithMessage($"Role must be one of the following: {string.Join(", ", Enum.GetNames(typeof(Role)))}")
                    .When(u => u.Role != null);
        }
    }
}
