using Application.Requests_Responses.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class CreateGuestRequestValidator : AbstractValidator<CreateGuestRequest>
{
    public CreateGuestRequestValidator()
    {
        RuleFor(x => x.PlayerName)
            .SetValidator(new PlayerNameValidator());

        RuleFor(x => x.Avatar)
            .SetValidator(new ImageValidator());
    }
}