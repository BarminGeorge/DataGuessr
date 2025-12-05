using Application.Requests_Responses.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Login)
            .SetValidator(new LoginValidator());

        RuleFor(x => x.Password)
            .SetValidator(new PasswordUserValidator());

        RuleFor(x => x.PlayerName)
            .SetValidator(new PlayerNameValidator());

        RuleFor(x => x.Avatar)
            .SetValidator(new ImageValidator());
    }
}