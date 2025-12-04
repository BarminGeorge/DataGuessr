using FluentValidation;

namespace Application.Requests_Responses.Validators.ParameterValidators;

public class RoomPasswordValidator : AbstractValidator<string?>
{
    public  RoomPasswordValidator()
    {
        RuleFor(x => x)
            .MinimumLength(4).WithMessage("Password must be at least 4 characters long")
            .MaximumLength(20).WithMessage("Password cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x));
    }
}