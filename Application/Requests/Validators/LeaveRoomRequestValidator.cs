using FluentValidation;

namespace Application.Requests.Validators;

public class LeaveRoomRequestValidator : AbstractValidator<LeaveRoomRequest>
{
    public LeaveRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required");
    }
}