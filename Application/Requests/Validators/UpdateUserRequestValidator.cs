using Application.Requests_Responses.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.PlayerName)
            .SetValidator(new PlayerNameValidator());
        
        RuleFor(x => x.Avatar)
            .SetValidator(new ImageValidator());
    }
}