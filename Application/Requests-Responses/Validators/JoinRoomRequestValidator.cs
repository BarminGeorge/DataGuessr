using Application.Requests_Responses.Validators.ParameterValidators;
using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class JoinRoomRequestValidator : AbstractValidator<JoinRoomRequest>
{
    public JoinRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required");

        RuleFor(x => x.Password)
            .SetValidator(new RoomPasswordValidator());
    }
}