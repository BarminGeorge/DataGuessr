using Application.Requests.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests.Validators;

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