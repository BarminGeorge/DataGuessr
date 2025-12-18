using FluentValidation;

namespace Application.Requests.Validators.ParameterValidators;

public class PlayerNameValidator : AbstractValidator<string>
{
    public PlayerNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Player name is required")
            .MinimumLength(2).WithMessage("Player name must be at least 2 characters long")
            .MaximumLength(30).WithMessage("Player name cannot exceed 30 characters")
            .Matches(@"^[а-яА-Яa-zA-Z0-9_\s]+$").WithMessage("Player name can only contain letters, numbers, spaces and underscores");
    }
}