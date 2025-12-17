using Application.Requests.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests.Validators;

public class GetRoomPrivacyRequestValidator : AbstractValidator<GetRoomPrivacyRequest>
{
    public GetRoomPrivacyRequestValidator()
    {
        RuleFor(x => x.InviteCode)
            .SetValidator(new InviteCodeValidator());
    }
}