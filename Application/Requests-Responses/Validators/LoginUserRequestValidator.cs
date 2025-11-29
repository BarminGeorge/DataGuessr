using Application.Requests_Responses.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Login)
            .SetValidator(new LoginValidator());

        RuleFor(x => x.Password)
            .SetValidator(new PasswordUserValidator());
    }
}