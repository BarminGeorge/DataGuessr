using FluentValidation;

namespace Application.Requests_Responses.Validators.ParameterValidators;

public class LoginValidator : AbstractValidator<string>
{
    public LoginValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Login is required")
            .MinimumLength(3).WithMessage("Login must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Login cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Login can only contain letters, numbers and underscores");
    }
}