using FluentValidation;

namespace Application.Requests_Responses.Validators.ParameterValidators;

public class PasswordUserValidator : AbstractValidator<string>
{
    public PasswordUserValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit");
    }
}