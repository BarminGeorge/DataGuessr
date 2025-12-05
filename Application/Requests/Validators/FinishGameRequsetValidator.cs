using FluentValidation;

namespace Application.Requests.Validators;

public class FinishGameRequsetValidator : AbstractValidator<FinishGameRequest>
{
    public FinishGameRequsetValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}