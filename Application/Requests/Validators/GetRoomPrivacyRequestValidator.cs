using FluentValidation;

namespace Application.Requests.Validators;

public class GetRoomPrivacyRequestValidator : AbstractValidator<GetRoomPrivacyRequest>
{
    public GetRoomPrivacyRequestValidator()
    {
        RuleFor(x => x.InviteCode)
            .NotEmpty()
            .WithMessage("InviteCode is required")
            .Length(4, 8)
            .WithMessage("InviteCode must be between 4 and 8 characters")
            .Matches("^[A-Z1-9]+$")
            .WithMessage("InviteCode can only contain uppercase letters (A-Z) and digits (1-9)");
    }
}