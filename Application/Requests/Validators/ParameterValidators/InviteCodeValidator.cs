using FluentValidation;

namespace Application.Requests.Validators.ParameterValidators;

public class InviteCodeValidator : AbstractValidator<string>
{
    public InviteCodeValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("InviteCode is required")
            .Length(4, 8)
            .WithMessage("InviteCode must be between 4 and 8 characters")
            .Matches("^[A-Z1-9]+$")
            .WithMessage("InviteCode can only contain uppercase letters (A-Z) and digits (1-9)");
    }
}