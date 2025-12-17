using Application.Requests.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests.Validators;

public class JoinRoomRequestValidator : AbstractValidator<JoinRoomRequest>
{
    public JoinRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.InviteCode)
            .SetValidator(new InviteCodeValidator());

        RuleFor(x => x.Password)
            .SetValidator(new RoomPasswordValidator());
    }
}